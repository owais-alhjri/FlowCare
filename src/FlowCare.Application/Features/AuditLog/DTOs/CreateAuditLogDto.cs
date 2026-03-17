using System.Text.Json;

namespace FlowCare.Application.Features.AuditLog.DTOs;

public class CreateAuditLogDto
{
    public string ActorId { get; init; } = null!;
    public string ActorRole { get; set; } = null!;
    public string ActionType { get; init; } = null!;
    public string EntityType { get; init; } = null!;
    public string EntityId { get; init; } = null!;
    public JsonDocument Metadata { get; init; } = null!;
}