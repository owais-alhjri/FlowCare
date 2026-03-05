using System.Text.Json.Serialization;

namespace FlowCare.Infrastructure.Persistence.SeedDtos;

public class BranchSeedDto
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("city")] public string City { get; set; }
    [JsonPropertyName("address")] public string Address { get; set; }
    [JsonPropertyName("timezone")] public string Timezone { get; set; }
    [JsonPropertyName("is_active")] public bool IsActive { get; set; }
}