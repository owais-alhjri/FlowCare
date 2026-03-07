using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Persistence;

public interface IAppointmentRepository
{
    Task CreateAppointment(Appointment appointment);
    Task<Appointment?> FetchLastId();
    Task SaveChangesAsync();
    Task<bool> IsSlotBooked(string slotId);
}