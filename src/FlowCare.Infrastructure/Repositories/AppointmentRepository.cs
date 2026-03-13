using FlowCare.Application.Interfaces;
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

    public async Task<List<Appointment>> AppointmentList(string userId)
    {
        var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException("User not found");

        var isAdmin = user.UserRole == UserRole.ADMIN;
        var isManager = user.UserRole == UserRole.BRANCH_MANAGER;


        return await dbContext.Appointments.Where(c => c.CustomerId == userId ||
                                                       c.StaffId == userId || isAdmin ||
                                                       (isManager && c.BranchId == user.BranchId))
            .Include(c => c.Customer)
            .Include(c => c.Staff)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Appointment?> FetchByAppointmentId(string appointmentId)
    {
        return await dbContext.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
    }

    public async Task<Appointment?> FetchByAppointmentIdAndRules(string appointmentId, string userId)
    {
        var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException("User not found");
        var isAdmin = user.UserRole == UserRole.ADMIN;
        var isManager = user.UserRole == UserRole.BRANCH_MANAGER;

        return await dbContext.Appointments
            .Where(c => c.Id == appointmentId && (
                c.StaffId == userId ||
                isAdmin ||
                (isManager && c.BranchId == user.BranchId)
            ))
            .Include(c => c.Customer)
            .Include(c => c.Staff)
            .FirstOrDefaultAsync();
    }

    public async Task<Appointment?> FetchAppointmentAttachment(string appointmentId, string userId)
    {
        var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException("User not found");
        var isAdmin = user.UserRole == UserRole.ADMIN;
        var isManager = user.UserRole == UserRole.BRANCH_MANAGER;
        var isStaff = user.UserRole == UserRole.STAFF;
        var isCustomer = user.UserRole == UserRole.CUSTOMER;

        return await dbContext.Appointments.Include(a => a.Staff)
            .Where(c => c.Id == appointmentId && (isAdmin ||
                                                  isManager && c.BranchId == user.BranchId
                                                  || isStaff && c.StaffId == user.Id
                                                  || isCustomer && c.CustomerId == user.Id
                )).AsNoTracking().FirstOrDefaultAsync();
    }
}