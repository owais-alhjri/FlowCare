using FlowCare.Application.Common;
using FlowCare.Application.Features.AuditLog.DTOs;
using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Services;

public class AuditLogService(IAuditLogRepository auditLogRepository, ICustomerRepository customerRepository)
{
    public async Task<Result<AuditLogResponseDto>> AddLog(CreateAuditLogDto dto)
    {
        var logPrefix = "aud_";
        var lastLog = await auditLogRepository.FetchLastLog();
        string fullId;
        if (lastLog is null)
            fullId = logPrefix + "001";
        else
        {
            var lastIdString = lastLog.Id.Substring(logPrefix.Length);
            var lastNumber = int.Parse(lastIdString);
            fullId = logPrefix + (lastNumber + 1).ToString("D3");
        }

        var user = await customerRepository.ExistIdAsync(dto.ActorId);
        if (user is null)
            return Result<AuditLogResponseDto>.Fail("User not found");

        var logs = new AuditLog(fullId, user, dto.ActionType, dto.EntityType, dto.EntityId, dto.Metadata);
        var auditLog = await auditLogRepository.AddLog(logs);

        return Result<AuditLogResponseDto>.Success(new AuditLogResponseDto
        {
            Id = auditLog.Id,
            ActionType = auditLog.ActionType,
            ActorId = auditLog.ActorId,
            ActorRole = auditLog.ActorRole,
            EntityId = auditLog.EntityId,
            EntityType = auditLog.EntityType,
            Timestamp = auditLog.Timestamp,
            Metadata = auditLog.Metadata,
        });
    }

    public async Task<Result<List<AuditLogResponseDto>>> ViewLogs(string userId)
    {
        var logs = await auditLogRepository.FetchLogs(userId);

        return Result<List<AuditLogResponseDto>>.Success(logs.Select(c => new AuditLogResponseDto
        {
            Id = c.Id,
            ActionType = c.ActionType,
            ActorId = c.ActorId,
            ActorRole = c.ActorRole,
            EntityId = c.EntityId,
            EntityType = c.EntityType,
            Timestamp = c.Timestamp,
            Metadata = c.Metadata
        }).ToList());
    }

    public async Task<Result<List<AuditLogCsvDto>>> AllLogs()
    {
        var logs = await auditLogRepository.AllLogs();

        return Result<List<AuditLogCsvDto>>.Success(logs.Select(log => new AuditLogCsvDto
        {
            Id = log.Id,
            ActionType = log.ActionType,
            ActorId = log.ActorId,
            ActorRole = log.ActorRole,
            EntityId = log.EntityId,
            EntityType = log.EntityType,
            Timestamp = log.Timestamp,
            Metadata = log.Metadata.RootElement.ToString()
        }).ToList());
    }
}