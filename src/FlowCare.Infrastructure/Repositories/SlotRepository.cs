using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;
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
}