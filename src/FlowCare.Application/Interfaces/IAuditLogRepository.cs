using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog> AddLog(AuditLog auditLog);
    Task<AuditLog?> GetLastLog();
    Task<List<AuditLog>> GetLogs(string userId);
    Task<List<AuditLog>> AllLogs();
}