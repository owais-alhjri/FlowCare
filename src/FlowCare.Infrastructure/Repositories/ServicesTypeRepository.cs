using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;
using FlowCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowCare.Infrastructure.Repositories;

public class ServicesTypeRepository(FlowCareDbContext dbContext) : IServicesTypeRepository
{
    public async Task<List<ServiceType>> ServicesByBranch(string branchId)
    {
        return await dbContext.ServiceTypes.Where(b=>b.BranchId == branchId).AsNoTracking().ToListAsync();
    }
}