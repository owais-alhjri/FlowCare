using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;
using FlowCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowCare.Infrastructure.Repositories;

public class ServicesTypeRepository(FlowCareDbContext dbContext) : IServicesTypeRepository
{
    public async Task<List<ServiceType>> ServicesByBranch(string branchId)
    {
        return await dbContext.ServiceTypes.Where(b => b.BranchId == branchId).AsNoTracking().ToListAsync();
    }

    public async Task<ServiceType?> FindByIdAsync(string id)
    {
        return await dbContext.ServiceTypes.FindAsync(id);
    }
}