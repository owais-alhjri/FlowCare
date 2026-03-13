using System.Text.Json.Serialization;

namespace FlowCare.Infrastructure.Persistence.SeedDtos;

public class SystemSettingSeedDto
{
    [JsonPropertyName("key")] public string Key { get; set; } = null!;
    [JsonPropertyName("value")] public string Value { get; set; } = null!;
    [JsonPropertyName("description")] public string? Description { get; set; }
}