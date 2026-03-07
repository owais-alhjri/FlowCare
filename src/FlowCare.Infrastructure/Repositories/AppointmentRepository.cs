using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;
using FlowCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowCare.Infrastructure.Repositories;

public class AppointmentRepository(FlowCareDbContext dbContext) : IAppointmentRepository
{
    public async Task CreateAppointment(Appointment appointment)
    {
         await dbContext.Appointments.AddAsync(appointment);
    }

    public async Task<Appointment?> FetchLastId()
    {
        return await dbContext.Appointments.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsSlotBooked(string slotId)
    {
        return await dbContext.Appointments
            .AnyAsync(a => a.SlotId == slotId && a.Status != Status.CANCELLED);
    }

    public async Task<List<Appointment>> AppointmentList(string customerId)
    {
        return await dbContext.Appointments.Where(c => c.CustomerId == customerId).AsNoTracking().ToListAsync();
    }

    public async Task<Appointment?> FetchById(string appointmentId)
    {
        return await dbContext.Appointments.FindAsync(appointmentId);
    }
}