using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;
using FlowCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowCare.Infrastructure.Repositories;

public class SlotRepository(FlowCareDbContext dbContext, IAppSettingRepository appSettingRepository) : ISlotsRepository
{
    public async Task<List<Slot>> SlotByBranchAndServiceType(string branchId, string serviceTypeId, DateTime? date)
    {
        var query = dbContext.Slots.Where(s => s.BranchId == branchId && s.ServiceTypeId == serviceTypeId);
        if (date.HasValue)
        {
            query = query.Where(s => s.StartedAt.Date == date.Value.Date);
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<Slot?> FindSlot(string slotId)
    {
        return await dbContext.Slots.Include(s => s.Staff).FirstOrDefaultAsync(s => s.Id == slotId);
    }

    public async Task CreateSlot(Slot slot)
    {
        await dbContext.Slots.AddAsync(slot);
    }

    public async Task<Slot?> GetLastId()
    {
        return await dbContext.Slots.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task<Slot?> GetBySlotId(string slotId, string userId)
    {
        var user = await dbContext.Users.FindAsync(userId);
        if (user is null) return null;
        var isAdmin = user.UserRole == UserRole.ADMIN;
        var isManager = user.UserRole == UserRole.BRANCH_MANAGER;

        var query = dbContext.Slots.AsQueryable();

        if (isAdmin)
        {
            query = query.IgnoreQueryFilters();
        }

        return await dbContext.Slots
            .Where(c => c.Id == slotId && (isAdmin ||
                                           (isManager && c.BranchId == user.BranchId))).FirstOrDefaultAsync();
    }

    public async Task<List<Slot>> SlotsByDeletedAt()
    {
        var appSetting = await appSettingRepository.GetRetentionPeriod();
        var retentionPeriod = appSetting.Value;
        var cutOffDate = DateTimeOffset.UtcNow.AddDays(-(int.Parse(retentionPeriod)));

        var slots = await dbContext.Slots.Where(s => s.DeletedAt != null && s.DeletedAt < cutOffDate)
            .IgnoreQueryFilters().ToListAsync();

        return slots;
    }

    public void RemoveSlot(Slot slot)
    {
        dbContext.Slots.Remove(slot);
    }
}