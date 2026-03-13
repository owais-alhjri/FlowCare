using System.Text.Json;
using FlowCare.Application.Features.Appointment.DTOs;
using FlowCare.Application.Features.AuditLog.DTOs;
using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;

namespace FlowCare.Application.Services;

public class AppointmentService(AuditLogService auditLogService, ISlotsRepository slotsRepository,
    IAppointmentRepository appointmentRepository,
    ICustomerRepository customerRepository, IStorageService storageService)
{
    // for the customer to book appointment
    public async Task BookAppointment(BookAppointmentDto bookAppointmentDto, string customerId)
    {

        var appointmentIdPiss = "appt_";
        var lastAppointment = await appointmentRepository.FetchLastId();

        string fullId;
        if (lastAppointment is null)
        {
            fullId = appointmentIdPiss + "001";
        }
        else
        {
            var lasIdString = lastAppointment.Id.Substring(appointmentIdPiss.Length);

            var lastNumber = int.Parse(lasIdString);
            var nextNumber = lastNumber + 1;

            fullId = appointmentIdPiss + nextNumber.ToString("D3");
        }

        var slotId = bookAppointmentDto.SlotId;

        var slot = await slotsRepository.FindSlot(slotId);
        if (slot == null)
            throw new ArgumentException("Slot not found");

        var alreadyBooked = await appointmentRepository.IsSlotBooked(slotId);
        if (alreadyBooked)
        {
            throw new ArgumentException("This slot is already booked");
        }

        if (slot.Staff == null)
            throw new ArgumentException("Slot has no assigned staff");

        var staff = await customerRepository.ExistsByStaffId(slot.Staff.Id)
                    ?? throw new ArgumentException("Staff is not available");

        var customer = await customerRepository.ExistIdAsync(customerId)
                       ?? throw new ArgumentException("Customer not found");


        string? dbReference = null;
        if (bookAppointmentDto.AttachmentPath != null)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(bookAppointmentDto.AttachmentPath.FileName)}";
            using var stream = bookAppointmentDto.AttachmentPath.OpenReadStream();
            dbReference = await storageService.UploadFileAsync(
                "appointment-attachments",
                fileName,
                stream,
                bookAppointmentDto.AttachmentPath.ContentType
            );
        }


        var appointment = new Appointment(fullId, customer, staff, slot.BranchId, slot.ServiceTypeId,slotId, Status.BOOKED,
            DateTimeOffset.UtcNow);

        if (dbReference != null)
            appointment.SetAttachmentsPath(dbReference);

        slot.ChangeActive(slot.IsActive);
        await appointmentRepository.CreateAppointment(appointment);
        await appointmentRepository.SaveChangesAsync();
        var log = new CreateAuditLogDto
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

        };
        await auditLogService.AddLog(log);
    }



    // this will list the appointment based on the user role
    // Admin → all branches
    // Manager → his branch only
    // Staff → assigned to him only
    // customer → his own appointment only
    public async Task<List<AppointmentResponseDto>> AppointmentById(string userId)
    {
        var appointmentList = await appointmentRepository.AppointmentList(userId);
        return appointmentList.Select(s=> new AppointmentResponseDto
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
    }

    // this is for customer to check appointment details 
    public async Task<AppointmentResponseDto?> AppointmentDetails(string appointmentId)
    {
        var appointment = await appointmentRepository.FetchByAppointmentId(appointmentId) ?? throw new ArgumentException("Appointment is not available");

        return new AppointmentResponseDto
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
        };
    }

    // this is for customer to cancel appointment
    public async Task<UpdateStatusOfAppointmentDto> CancelAppointment(string appointmentId, string customerId)
    {
        var appointment = await appointmentRepository.FetchByAppointmentId(appointmentId)?? throw new ArgumentException("Appointment is not available");
        var customer = await customerRepository.ExistIdAsync(customerId) ?? throw new ArgumentException("Customer not found");
        appointment.CanceledAppointment();
        await appointmentRepository.SaveChangesAsync();

        var log = new CreateAuditLogDto
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

        };
        await auditLogService.AddLog(log);
        return new UpdateStatusOfAppointmentDto
        {
            Id = appointment.Id,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
        };


    }

    // this is for customer to reschedule appointment
    public async Task<RescheduleAppointmenDto> Reschedule(string appointmentId, string slotId, string customerId)
    {
        var appointment = await appointmentRepository.FetchByAppointmentId(appointmentId) ?? throw new ArgumentException("Appointment is not available");
        var customer = await customerRepository.ExistIdAsync(customerId) ?? throw new ArgumentException("Customer not found");
        appointment.RescheduleAppointmentSlot(appointment.SlotId, slotId);
        await appointmentRepository.SaveChangesAsync();
        var log = new CreateAuditLogDto
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

        };
        await auditLogService.AddLog(log);
        return new RescheduleAppointmenDto
        {
            Id = appointment.Id,
            SlotId = slotId,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
        };
    }
    // Update appointment status (checked-in, no-show, completed)
    public async Task<UpdateStatusOfAppointmentDto> UpdateAppointmentStatus(string appointmentId, string userId, string state )
    {
        var appointment = await appointmentRepository.FetchByAppointmentIdAndRules(appointmentId,userId)
                          ?? throw new ArgumentException("Appointment is not available");
        var staff = await customerRepository.ExistsByStaffId(userId) ?? throw new ArgumentException("Staff not found");

        if (state == "BOOKED" || state == "RESCHEDULE" && userId == staff.Id)
        {
            throw new ArgumentException("Staff can not change status to BOOKED OR RESCHEDULE");
        }
        appointment.UpdateAppointmentStatus(appointment.Status.ToString(),state);

        await appointmentRepository.SaveChangesAsync();
        var type = state.ToUpper();
        var log = new CreateAuditLogDto
        {
            ActorId = staff.Id,
            ActorRole = staff.UserRole.ToString(),
            ActionType = "APPOINTMENT_" + type ,
            EntityType = "APPOINTMENT",
            EntityId = appointment.Id,
            Metadata = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                slot_id = appointment.SlotId,
                branch_id = appointment.BranchId,
                service_type_id = appointment.ServiceTypeId

            }))

        };
        await auditLogService.AddLog(log);

        return new UpdateStatusOfAppointmentDto
        {
            Id = appointment.Id,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
        };
    }

}