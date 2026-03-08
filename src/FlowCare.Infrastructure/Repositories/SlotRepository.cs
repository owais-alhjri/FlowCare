using FlowCare.Application.Features.Slot.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;
using FlowCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowCare.Infrastructure.Repositories;

public class SlotRepository(FlowCareDbContext dbContext): ISlotsRepository
{

    public async Task<List<Slot>> SlotByBranchAndServiceType(string branchId, string serviceTypeId, DateTime? date)
    {
        var query =  dbContext.Slots.Where(s => s.BranchId == branchId && s.ServiceTypeId == serviceTypeId);
        if (date.HasValue)
        {
            query = query.Where(s => s.StartedAt.Date == date.Value.Date);
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<Slot?> FindSlot(string slotId)
    {
        return await dbContext.Slots.Include(s=>s.Staff).FirstOrDefaultAsync(s=>s.Id == slotId);
    }

    public async Task CreateSlot(Slot slot )
    {
         await dbContext.Slots.AddAsync(slot);
    }

    public async Task<Slot?> FetchLastId()
    {
        return await dbContext.Slots.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task<Slot?> FetchBySlotId(string slotId, string userId)
    {
        var user = await dbContext.Users.FindAsync(userId);
        if (user is null) return null;
        var isAdmin = user.UserRole == UserRole.ADMIN;
        var isManager = user.UserRole == UserRole.BRANCH_MANAGER;

        return await dbContext.Slots
            .Where(c => c.Id == slotId && ( isAdmin ||
                                           (isManager && c.BranchId == user.BranchId))).FirstOrDefaultAsync();
    }

}