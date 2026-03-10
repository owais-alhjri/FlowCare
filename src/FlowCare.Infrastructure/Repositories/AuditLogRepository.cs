using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;
using FlowCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowCare.Infrastructure.Repositories;

public class AuditLogRepository(FlowCareDbContext dbContext) : IAuditLogRepository
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

}