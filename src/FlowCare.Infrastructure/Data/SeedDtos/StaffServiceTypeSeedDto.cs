using System.Text.Json.Serialization;

namespace FlowCare.Infrastructure.Data.SeedDtos;

public class StaffServiceTypeSeedDto
{
    [JsonPropertyName("staff_id")] public string StaffId { get; set; } = null!;
    [JsonPropertyName("service_type_id")] public string ServiceTypeId { get; set; } = null!;
}