using System.Text.Json.Serialization;

namespace FlowCare.Infrastructure.Persistence.SeedDtos;

public class SystemSettingSeedDto
{
    [JsonPropertyName("key")] public string Key { get; set; }
    [JsonPropertyName("value")] public string Value { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }

}