using System.Text.Json.Serialization;

namespace FlowCare.Infrastructure.Persistence.SeedDtos;

public class ServiceTypeSeedDto
{
    [JsonPropertyName("id")] public string Id { get; set; } = null!;
    [JsonPropertyName("branch_id")] public string BranchId { get; set; } = null!;
    [JsonPropertyName("name")] public string Name { get; set; } = null!;
    [JsonPropertyName("description")] public string Description { get; set; } = null!;
    [JsonPropertyName("duration_minutes")] public int DurationMinutes { get; set; }
    [JsonPropertyName("is_active")] public bool IsActive { get; set; }
}