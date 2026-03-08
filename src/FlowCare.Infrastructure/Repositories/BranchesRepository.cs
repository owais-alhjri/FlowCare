using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;
using FlowCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowCare.Infrastructure.Repositories;

public class BranchesRepository(FlowCareDbContext dbContext) : IBranchesRepository
{

    public async Task<List<Branch>> BranchesList()
    {
        // Adding AsNoTracking will make the EF give the data without tracking it | it will make it faster
        return await dbContext.Branches.AsNoTracking().ToListAsync();
    }

    public async Task<Branch?> FindById(string branchId)
    {
        return await dbContext.Branches.FindAsync(branchId);
    }
}