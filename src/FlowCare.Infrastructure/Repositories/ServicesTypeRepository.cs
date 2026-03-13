using System;
using FlowCare.Application.Interfaces;
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

    public async Task<ServiceType?> ExistIdAsync(string id)
    {
        return await dbContext.ServiceTypes.FindAsync(id);

    }

}