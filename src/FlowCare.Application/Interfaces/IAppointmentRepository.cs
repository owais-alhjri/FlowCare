using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces;

public interface IAppointmentRepository
{
    Task CreateAppointment(Appointment appointment);
    Task<Appointment?> GetLastId();
    Task SaveChangesAsync();
    Task<bool> IsSlotBooked(string slotId);
    Task<List<Appointment>> AppointmentList(string userId);
    Task<Appointment?> GetByAppointmentId(string appointmentId);
    Task<Appointment?> GetByAppointmentIdAndRules(string appointmentId, string userId);
    Task<Appointment?> GetAppointmentAttachment(string appointmentId, string userId);
    Task<Appointment?> GetLastQueueByBranch(string branchId);
}