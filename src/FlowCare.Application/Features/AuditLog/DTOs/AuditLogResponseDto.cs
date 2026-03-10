using System.Text.Json;

namespace FlowCare.Application.Features.AuditLog.DTOs;

public class AuditLogResponseDto
{
    public string Id { get; set; } = null!;
    public string ActorId { get; set; } = null!;
    public string ActorRole { get; set; } = null!;
    public string ActionType { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public DateTimeOffset Timestamp { get; set; }
    public JsonDocument Metadata { get; set; } = null!;
}