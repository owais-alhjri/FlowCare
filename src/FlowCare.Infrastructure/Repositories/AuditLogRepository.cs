using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;
using FlowCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowCare.Infrastructure.Repositories;

public class AuditLogRepository(FlowCareDbContext dbContext ) : IAuditLogRepository
{
    public async Task<AuditLog> AddLog(AuditLog auditLog)
    {
        var logs = await dbContext.AuditLogs.AddAsync(auditLog);
        await dbContext.SaveChangesAsync();
        return logs.Entity;
    }

    public async Task<AuditLog?> FetchLastLog()
    {
       return await dbContext.AuditLogs.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
    }

    public async Task<List<AuditLog>> FetchLogs(string userId)
    {
        var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException("User not found");
        var isAdmin = user.UserRole == UserRole.ADMIN;
        var isManager = user.UserRole == UserRole.BRANCH_MANAGER;

        var logs = await dbContext.AuditLogs.ToListAsync();

        return logs.Where(c =>
            isAdmin ||
            (isManager && c.Metadata.RootElement.TryGetProperty("branch_id", out var branchIdElement)
                       && branchIdElement.GetString() == user.BranchId)
        ).ToList();
    }

    public async Task<List<AuditLog>> AllLogs()
    {
        return await dbContext.AuditLogs.ToListAsync();
    }
}