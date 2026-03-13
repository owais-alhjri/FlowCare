using System.Text.Json;
using FlowCare.Application.Common;
using FlowCare.Application.Features.Appointment.DTOs;
using FlowCare.Application.Features.AuditLog.DTOs;
using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;

namespace FlowCare.Application.Services;

public class AppointmentService(
    AuditLogService auditLogService,
    ISlotsRepository slotsRepository,
    IAppointmentRepository appointmentRepository,
    ICustomerRepository customerRepository,
    IStorageService storageService)
{
    public async Task<Result> BookAppointment(BookAppointmentDto bookAppointmentDto, string customerId)
    {
        var appointmentIdPrefix = "appt_";
        var lastAppointment = await appointmentRepository.FetchLastId();

        string fullId;
        if (lastAppointment is null)
            fullId = appointmentIdPrefix + "001";
        else
        {
            var lastIdString = lastAppointment.Id.Substring(appointmentIdPrefix.Length);
            var lastNumber = int.Parse(lastIdString);
            fullId = appointmentIdPrefix + (lastNumber + 1).ToString("D3");
        }

        var slot = await slotsRepository.FindSlot(bookAppointmentDto.SlotId);
        if (slot is null)
            return Result.Fail("Slot not found");

        if (await appointmentRepository.IsSlotBooked(bookAppointmentDto.SlotId))
            return Result.Fail("This slot is already booked");

        if (slot.Staff is null)
            return Result.Fail("Slot has no assigned staff");

        var staff = await customerRepository.ExistsByStaffId(slot.Staff.Id);
        if (staff is null)
            return Result.Fail("Staff is not available");

        var customer = await customerRepository.ExistIdAsync(customerId);
        if (customer is null)
            return Result.Fail("Customer not found");

        string? dbReference = null;
        if (bookAppointmentDto.AttachmentPath != null)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(bookAppointmentDto.AttachmentPath.FileName)}";
            using var stream = bookAppointmentDto.AttachmentPath.OpenReadStream();
            dbReference = await storageService.UploadFileAsync(
                "appointment-attachments", fileName, stream, bookAppointmentDto.AttachmentPath.ContentType);
        }

        var appointment = new Appointment(fullId, customer, staff, slot.BranchId, slot.ServiceTypeId,
            bookAppointmentDto.SlotId, Status.BOOKED, DateTimeOffset.UtcNow);

        if (dbReference != null)
            appointment.SetAttachmentsPath(dbReference);

        slot.ChangeActive(slot.IsActive);
        await appointmentRepository.CreateAppointment(appointment);
        await appointmentRepository.SaveChangesAsync();

        await auditLogService.AddLog(new CreateAuditLogDto
        {
            ActorId = customer.Id,
            ActorRole = customer.UserRole.ToString(),
            ActionType = "APPOINTMENT_BOOKED",
            EntityType = "APPOINTMENT",
            EntityId = appointment.Id,
            Metadata = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                slot_id = appointment.SlotId,
                branch_id = appointment.BranchId,
                service_type_id = appointment.ServiceTypeId
            }))
        });

        return Result.Success();
    }

    public async Task<Result<List<AppointmentResponseDto>>> AppointmentById(string userId)
    {
        var appointmentList = await appointmentRepository.AppointmentList(userId);
        var result = appointmentList.Select(s => new AppointmentResponseDto
        {
            Id = s.Id,
            BranchId = s.BranchId,
            CustomerId = s.CustomerId,
            ServiceTypeId = s.ServiceTypeId,
            SlotId = s.SlotId,
            StaffId = s.StaffId,
            Status = s.Status,
            CreatedAt = s.CreatedAt
        }).ToList();

        return Result<List<AppointmentResponseDto>>.Success(result);
    }

    public async Task<Result<AppointmentResponseDto>> AppointmentDetails(string appointmentId)
    {
        var appointment = await appointmentRepository.FetchByAppointmentId(appointmentId);
        if (appointment is null)
            return Result<AppointmentResponseDto>.Fail("Appointment is not available");

        return Result<AppointmentResponseDto>.Success(new AppointmentResponseDto
        {
            Id = appointment.Id,
            BranchId = appointment.BranchId,
            CustomerId = appointment.CustomerId,
            ServiceTypeId = appointment.ServiceTypeId,
            SlotId = appointment.SlotId,
            StaffId = appointment.StaffId,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
            AttachmentPath = appointment.AttachmentPath
        });
    }

    public async Task<Result<UpdateStatusOfAppointmentDto>> CancelAppointment(string appointmentId, string customerId)
    {
        var appointment = await appointmentRepository.FetchByAppointmentId(appointmentId);
        if (appointment is null)
            return Result<UpdateStatusOfAppointmentDto>.Fail("Appointment is not available");

        if (appointment.CustomerId != customerId)
            return Result<UpdateStatusOfAppointmentDto>.Fail("Unauthorized");

        var customer = await customerRepository.ExistIdAsync(customerId);
        if (customer is null)
            return Result<UpdateStatusOfAppointmentDto>.Fail("Customer not found");


        appointment.CanceledAppointment();
        await appointmentRepository.SaveChangesAsync();

        await auditLogService.AddLog(new CreateAuditLogDto
        {
            ActorId = customer.Id,
            ActorRole = customer.UserRole.ToString(),
            ActionType = "APPOINTMENT_CANCELED",
            EntityType = "APPOINTMENT",
            EntityId = appointment.Id,
            Metadata = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                slot_id = appointment.SlotId,
                branch_id = appointment.BranchId,
                service_type_id = appointment.ServiceTypeId
            }))
        });

        return Result<UpdateStatusOfAppointmentDto>.Success(new UpdateStatusOfAppointmentDto
        {
            Id = appointment.Id,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
        });
    }

    public async Task<Result<RescheduleAppointmenDto>> Reschedule(string appointmentId, string slotId,
        string customerId)
    {
        var appointment = await appointmentRepository.FetchByAppointmentId(appointmentId);
        if (appointment is null)
            return Result<RescheduleAppointmenDto>.Fail("Appointment is not available");

        var customer = await customerRepository.ExistIdAsync(customerId);
        if (customer is null)
            return Result<RescheduleAppointmenDto>.Fail("Customer not found");

        appointment.RescheduleAppointmentSlot(appointment.SlotId, slotId);
        await appointmentRepository.SaveChangesAsync();

        await auditLogService.AddLog(new CreateAuditLogDto
        {
            ActorId = customer.Id,
            ActorRole = customer.UserRole.ToString(),
            ActionType = "APPOINTMENT_RESCHEDULE",
            EntityType = "APPOINTMENT",
            EntityId = appointment.Id,
            Metadata = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                slot_id = appointment.SlotId,
                branch_id = appointment.BranchId,
                service_type_id = appointment.ServiceTypeId
            }))
        });

        return Result<RescheduleAppointmenDto>.Success(new RescheduleAppointmenDto
        {
            Id = appointment.Id,
            SlotId = slotId,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
        });
    }

    public async Task<Result<UpdateStatusOfAppointmentDto>> UpdateAppointmentStatus(string appointmentId, string userId,
        string state)
    {
        var appointment = await appointmentRepository.FetchByAppointmentIdAndRules(appointmentId, userId);
        if (appointment is null)
            return Result<UpdateStatusOfAppointmentDto>.Fail("Appointment is not available");

        var staff = await customerRepository.ExistsByStaffId(userId);
        if (staff is null)
            return Result<UpdateStatusOfAppointmentDto>.Fail("Staff not found");

        if ((state == "BOOKED" || state == "RESCHEDULE") && userId == staff.Id)
            return Result<UpdateStatusOfAppointmentDto>.Fail("Staff cannot change status to BOOKED or RESCHEDULE");

        appointment.UpdateAppointmentStatus(appointment.Status.ToString(), state);
        await appointmentRepository.SaveChangesAsync();

        await auditLogService.AddLog(new CreateAuditLogDto
        {
            ActorId = staff.Id,
            ActorRole = staff.UserRole.ToString(),
            ActionType = "APPOINTMENT_" + state.ToUpper(),
            EntityType = "APPOINTMENT",
            EntityId = appointment.Id,
            Metadata = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                slot_id = appointment.SlotId,
                branch_id = appointment.BranchId,
                service_type_id = appointment.ServiceTypeId
            }))
        });

        return Result<UpdateStatusOfAppointmentDto>.Success(new UpdateStatusOfAppointmentDto
        {
            Id = appointment.Id,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
        });
    }

    public async Task<Result<AppointmentResponseDto>> GetAppointmentAttachment(string appointmentId, string userId)
    {
        var appointment = await appointmentRepository.FetchAppointmentAttachment(appointmentId, userId);
        if (appointment is null)
            return Result<AppointmentResponseDto>.Fail("Appointment not found");

        return Result<AppointmentResponseDto>.Success(new AppointmentResponseDto
        {
            Id = appointment.Id,
            BranchId = appointment.BranchId,
            CustomerId = appointment.CustomerId,
            ServiceTypeId = appointment.ServiceTypeId,
            SlotId = appointment.SlotId,
            StaffId = appointment.StaffId,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
            AttachmentPath = appointment.AttachmentPath
        });
    }
}