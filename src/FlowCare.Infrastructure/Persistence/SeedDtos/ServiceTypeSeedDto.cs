using System.Text.Json.Serialization;

namespace FlowCare.Infrastructure.Persistence.SeedDtos;

public class ServiceTypeSeedDto
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("branch_id")] public string BranchId { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("description")] public string Description { get; set; }
    [JsonPropertyName("duration_minutes")] public int DurationMinutes { get; set; }
    [JsonPropertyName("is_active")] public bool IsActive { get; set; }
}