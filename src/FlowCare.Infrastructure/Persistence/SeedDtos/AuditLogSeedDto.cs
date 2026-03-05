using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowCare.Infrastructure.Persistence.SeedDtos;

public class AuditLogSeedDto
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("actor_id")] public string ActorId { get; set; }
    [JsonPropertyName("actor_role")] public string ActorRole { get; set; }
    [JsonPropertyName("action_type")] public string ActionType { get; set; }
    [JsonPropertyName("entity_type")] public string EntityType { get; set; }
    [JsonPropertyName("entity_id")] public string EntityId { get; set; }
    [JsonPropertyName("timestamp")] public DateTimeOffset Timestamp { get; set; }
    [JsonPropertyName("metadata")] public JsonDocument Metadata { get; set; }
}