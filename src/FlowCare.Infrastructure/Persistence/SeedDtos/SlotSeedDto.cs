using System.Text.Json.Serialization;

namespace FlowCare.Infrastructure.Persistence.SeedDtos;

public class SlotSeedDto
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("branch_id")] public string BranchId { get; set; }
    [JsonPropertyName("service_type_id")] public string ServiceTypeId { get; set; }
    [JsonPropertyName("staff_id")] public string StaffId { get; set; }
    [JsonPropertyName("start_at")] public DateTimeOffset StartAt { get; set; }
    [JsonPropertyName("end_at")] public DateTimeOffset EndAt { get; set; }
    [JsonPropertyName("capacity")] public int Capacity { get; set; }
    [JsonPropertyName("is_active")] public bool IsActive { get; set; }
}