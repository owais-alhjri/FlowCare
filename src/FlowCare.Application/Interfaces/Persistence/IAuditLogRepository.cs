using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Persistence;

public interface IAuditLogRepository
{
    Task<AuditLog> AddLog(AuditLog auditLog);
    Task<AuditLog?> FetchLastLog();
    Task<List<AuditLog>> FetchLogs(string userId);
}