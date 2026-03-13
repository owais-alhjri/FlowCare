using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces;

public interface IAppointmentRepository
{
    Task CreateAppointment(Appointment appointment);
    Task<Appointment?> FetchLastId();
    Task SaveChangesAsync();
    Task<bool> IsSlotBooked(string slotId);
    Task<List<Appointment>> AppointmentList(string userId);
    Task<Appointment?> FetchByAppointmentId(string appointmentId);
    Task<Appointment?> FetchByAppointmentIdAndRules(string appointmentId, string userId);
}