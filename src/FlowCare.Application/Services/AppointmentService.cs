using System.Collections.Generic;
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
    IStorageService storageService,
    IAppSettingRepository appSettingRepository)
{
    public async Task<Result<AppointmentResponseDto>> BookAppointment(BookAppointmentDto bookAppointmentDto, string customerId)
    {

        var appointmentsPerDay = await appointmentRepository.GetAppointmentsPerDay(customerId);

        var appSetting = await appSettingRepository.GetBookingLimitPerDay();
        var limit = int.Parse(appSetting.Value);
        if (appointmentsPerDay >= limit)
        {
            return Result<AppointmentResponseDto>.Fail($"You can not have more then {limit} appointments per day");
        }


        var appointmentIdPrefix = "appt_";
        var lastAppointment = await appointmentRepository.GetLastId();

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
            return Result<AppointmentResponseDto>.Fail("Slot not found");

        if (await appointmentRepository.IsSlotBooked(bookAppointmentDto.SlotId))
            return Result<AppointmentResponseDto>.Fail("This slot is already booked");

        if (slot.Staff is null)
            return Result<AppointmentResponseDto>.Fail("Slot has no assigned staff");

        var staff = await customerRepository.FindByStaffId(slot.Staff.Id);
        if (staff is null)
            return Result<AppointmentResponseDto>.Fail("Staff is not available");

        var customer = await customerRepository.FindByIdAsync(customerId);
        if (customer is null)
            return Result<AppointmentResponseDto>.Fail("Customer not found");

        string? dbReference = null;
        if (bookAppointmentDto.AttachmentPath != null)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(bookAppointmentDto.AttachmentPath.FileName)}";
            await using var stream = bookAppointmentDto.AttachmentPath.OpenReadStream();
            dbReference = await storageService.UploadFileAsync(
                "appointment-attachments", fileName, stream, bookAppointmentDto.AttachmentPath.ContentType);
        }

        var lastQueue = await appointmentRepository.GetLastQueueByBranch(slot.BranchId, fullId);
        var nextQueue = (lastQueue?.Queue ?? 0) + 1;

        var appointment = new Appointment(fullId, customer, staff, slot.BranchId, slot.ServiceTypeId,
            bookAppointmentDto.SlotId, Status.BOOKED, DateTimeOffset.UtcNow, nextQueue);

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
        var appointment = await appointmentRepository.GetByAppointmentId(appointmentId);
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
        var appointment = await appointmentRepository.GetByAppointmentId(appointmentId);
        if (appointment is null)
            return Result<UpdateStatusOfAppointmentDto>.Fail("Appointment is not available");

        if (appointment.CustomerId != customerId)
            return Result<UpdateStatusOfAppointmentDto>.Fail("Unauthorized");

        var customer = await customerRepository.FindByIdAsync(customerId);
        if (customer is null)
            return Result<UpdateStatusOfAppointmentDto>.Fail("Customer not found");

        var currentQueue = appointment.Queue; 

        var lastQueue = await appointmentRepository.GetLastQueueByBranch(appointment.BranchId, appointment.Id); 
        for (int i = currentQueue + 1; i <= lastQueue.Queue; i++)
        {
            var queue = await appointmentRepository.FindByQueue(i, appointment.BranchId, appointment.Id);
            queue.ReduceQueue();
        }


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
        var frequencyLimitPerDay = await appSettingRepository.GetReschedulingLimitPerDay();
        var numberOfDays = int.Parse(frequencyLimitPerDay.Value);
        var frequencyRescheduling = await appointmentRepository.RescheduledTodayAsync(appointmentId, numberOfDays);

        if (!frequencyRescheduling)
            return Result<RescheduleAppointmenDto>.Fail($"No Rescheduling more then {numberOfDays} per day ");

        var appointment = await appointmentRepository.GetByAppointmentId(appointmentId);
        if (appointment is null)
            return Result<RescheduleAppointmenDto>.Fail("Appointment is not available");

        var customer = await customerRepository.FindByIdAsync(customerId);
        if (customer is null)
            return Result<RescheduleAppointmenDto>.Fail("Customer not found");

        var currentQueue = appointment.Queue; 

        var lastQueue = (await appointmentRepository.GetLastQueueByBranch(appointment.BranchId, appointment.Id)).Queue; 
        Console.WriteLine("last before "+lastQueue);

        //this is for if the rescheduled already the last one in the queue
        if (currentQueue >= lastQueue)
        {
            lastQueue = currentQueue;
        }

        for (int i = currentQueue + 1; i <= lastQueue; i++)
        {
            var queue = await appointmentRepository.FindByQueue(i, appointment.BranchId, appointment.Id);
            Console.WriteLine("queue Id " + queue.Id);
            Console.WriteLine("queue  " + queue.Queue);
            queue.ReduceQueue();
        }

        //this will set the rescheduled queue to the last 
        appointment.AddQueueBack(lastQueue);
        Console.WriteLine("last after " + lastQueue);


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
        var appointment = await appointmentRepository.GetByAppointmentIdAndRules(appointmentId, userId);
        if (appointment is null)
            return Result<UpdateStatusOfAppointmentDto>.Fail("Appointment is not available");

        var staff = await customerRepository.FindByStaffId(userId);
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
        var appointment = await appointmentRepository.GetAppointmentAttachment(appointmentId, userId);
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

    public async Task<Result<List<AppointmentQueueDto>>> GetAppointmentQueue(string userId)
    {
        var appointments = await appointmentRepository.AppointmentListForQueue(userId);
        var result = appointments.Select(a => new AppointmentQueueDto
        {
            Id = a.Id,
            BranchId = a.BranchId,
            CustomerId = a.CustomerId,
            CreatedAt = a.CreatedAt,
            Status = a.Status,
            Queue = a.Queue
        }).ToList();

        return Result<List<AppointmentQueueDto>>.Success(result);
    }
}