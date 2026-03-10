using FlowCare.Application.Features.AuditLog.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Services;

public class AuditLogService(IAuditLogRepository auditLogRepository, ICustomerRepository customerRepository)
{

    public async Task<AuditLogResponseDto> AddLog(CreateAuditLogDto dto)
    {
        var user = await customerRepository.ExistIdAsync(dto.ActorId)?? throw new ArgumentException("User not found");
        var logs = new AuditLog(dto.Id, user, dto.ActionType, dto.EntityType, dto.EntityId, dto.Metadata);
        var auditLog = await auditLogRepository.AddLog(logs);
        return new AuditLogResponseDto
        {
            Id = auditLog.Id,
            ActionType = auditLog.ActionType,
            ActorId = auditLog.ActorId,
            ActorRole = auditLog.ActorRole,
            EntityId = auditLog.EntityId,
            EntityType = auditLog.EntityType,
            Timestamp = auditLog.Timestamp,
            Metadata = auditLog.Metadata
        } ;
    }
}