using System.Text.Json.Serialization;

namespace FlowCare.Infrastructure.Persistence.SeedDtos;

public class BranchSeedDto
{
    [JsonPropertyName("id")] public string Id { get; set; } = null!;
    [JsonPropertyName("name")] public string Name { get; set; } = null!;
    [JsonPropertyName("city")] public string City { get; set; } = null!;
    [JsonPropertyName("address")] public string Address { get; set; } = null!;
    [JsonPropertyName("timezone")] public string Timezone { get; set; } = null!;
    [JsonPropertyName("is_active")] public bool IsActive { get; set; } 
}