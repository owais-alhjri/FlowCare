using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowCare.Infrastructure.Persistence.SeedDtos;

public class AuditLogSeedDto
{
    [JsonPropertyName("id")] public string Id { get; set; } = null!;
    [JsonPropertyName("actor_id")] public string ActorId { get; set; } = null!;
    [JsonPropertyName("actor_role")] public string ActorRole { get; set; } = null!;
    [JsonPropertyName("action_type")] public string ActionType { get; set; } = null!;
    [JsonPropertyName("entity_type")] public string EntityType { get; set; } = null!;
    [JsonPropertyName("entity_id")] public string EntityId { get; set; } = null!;
    [JsonPropertyName("timestamp")] public DateTimeOffset Timestamp { get; set; }
    [JsonPropertyName("metadata")] public JsonDocument Metadata { get; set; } = null!;
}