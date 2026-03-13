using FlowCare.Application.Features.AuditLog.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Services;

public class AuditLogService(IAuditLogRepository auditLogRepository, ICustomerRepository customerRepository)
{

    public async Task<AuditLogResponseDto> AddLog(CreateAuditLogDto dto)
    {

        var logPiss = "aud_";
        var lastLog = await auditLogRepository.FetchLastLog();

        string fullId;
        if (lastLog is null)
        {
            fullId = logPiss + "001";
        }
        else
        {
            var lasIdString = lastLog.Id.Substring(logPiss.Length);

            var lastNumber = int.Parse(lasIdString);
            var nextNumber = lastNumber + 1;

            fullId = logPiss + nextNumber.ToString("D3");
        }
        var user = await customerRepository.ExistIdAsync(dto.ActorId) ?? throw new ArgumentException("User not found");
        var logs = new AuditLog(fullId, user, dto.ActionType, dto.EntityType, dto.EntityId, dto.Metadata);
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
            Metadata = auditLog.Metadata,
        } ;
    }

    public async Task<List<AuditLogResponseDto>> ViewLogs(string userId)
    {

        var logs = await auditLogRepository.FetchLogs(userId);
        

        return logs.Select(c => new AuditLogResponseDto
        {
            Id = c.Id,
            ActionType = c.ActionType,
            ActorId = c.ActorId,
            ActorRole = c.ActorRole,
            EntityId = c.EntityId,
            EntityType = c.EntityType,
            Timestamp = c.Timestamp,
            Metadata = c.Metadata
        }).ToList();
    }

    public async Task<List<AuditLogCsvDto>> AllLogs()
    {
        var logs = await auditLogRepository.AllLogs();
        return logs.Select(log => new AuditLogCsvDto
        {
            Id = log.Id,
            ActionType = log.ActionType,
            ActorId = log.ActorId,
            ActorRole = log.ActorRole,
            EntityId = log.EntityId,
            EntityType = log.EntityType,
            Timestamp = log.Timestamp,
            Metadata = log.Metadata.RootElement.ToString()
        }).ToList();
    } 

}