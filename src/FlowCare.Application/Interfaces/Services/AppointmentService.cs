using FlowCare.Application.Features.Appointment.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;

namespace FlowCare.Application.Interfaces.Services;

public class AppointmentService(ISlotsRepository slotsRepository, IAppointmentRepository appointmentRepository, ICustomerRepository customerRepository)
{
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

        var appointment = new Appointment(fullId, customer, staff, slot.BranchId, slot.ServiceTypeId,slotId, Status.BOOKED,
            DateTimeOffset.UtcNow);

        await appointmentRepository.CreateAppointment(appointment);
        await appointmentRepository.SaveChangesAsync();
    }

    public async Task<List<AppointmentResponseDto>> AppointmentById(string customerId)
    {
        var appointmentList = await appointmentRepository.AppointmentList(customerId);
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

    public async Task<AppointmentResponseDto?> AppointmentDetails(string appointmentId)
    {
        var appointment = await appointmentRepository.FetchById(appointmentId) ?? throw new ArgumentException("Appointment is not available");

        return new AppointmentResponseDto
        {
            Id = appointment.Id,
            BranchId = appointment.BranchId,
            CustomerId = appointment.CustomerId,
            ServiceTypeId = appointment.ServiceTypeId,
            SlotId = appointment.SlotId,
            StaffId = appointment.StaffId,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt
        };
    }

    public async Task<CancelAppointmentDto> CancelAppointment(string appointmentId)
    {
        var appointment = await appointmentRepository.FetchById(appointmentId)?? throw new ArgumentException("Appointment is not available");

        appointment.CanceledAppointment();
        await appointmentRepository.SaveChangesAsync();
        return new CancelAppointmentDto
        {
            Id = appointment.Id,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
        };
    }

    public async Task<RescheduleAppointmenDto> Reschedule(string appointmentId, string slotId)
    {
        var appointment = await appointmentRepository.FetchById(appointmentId) ?? throw new ArgumentException("Appointment is not available");

        appointment.RescheduleAppointmentSlot(appointment.SlotId, slotId);
        await appointmentRepository.SaveChangesAsync();
        return new RescheduleAppointmenDto
        {
            Id = appointment.Id,
            SlotId = slotId,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
        };
    }

}