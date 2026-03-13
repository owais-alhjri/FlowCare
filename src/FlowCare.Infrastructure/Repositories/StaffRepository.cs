using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;
using FlowCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowCare.Infrastructure.Repositories;

public class StaffRepository(FlowCareDbContext dbContext) : IStaffRepository
{
    public async Task<List<User>> FetchStaff(string userId)
    {
        var user = await dbContext.Users.FindAsync(userId);

        if (user == null)
            throw new UnauthorizedAccessException("User Not Found");
        if (user.UserRole == UserRole.ADMIN)
        {
            return await dbContext.Users.Where(s => s.UserRole == UserRole.STAFF).AsNoTracking().ToListAsync();
        }

        if (user.UserRole == UserRole.BRANCH_MANAGER)
        {
            return await dbContext.Users.Where(s => s.UserRole == UserRole.STAFF && s.BranchId == user.BranchId)
                .AsNoTracking().ToListAsync();
        }

        throw new UnauthorizedAccessException("Only admin or manager can fetch staff");
    }
}