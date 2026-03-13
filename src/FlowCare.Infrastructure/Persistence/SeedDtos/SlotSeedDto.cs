using System.Text.Json.Serialization;

namespace FlowCare.Infrastructure.Persistence.SeedDtos;

public class SlotSeedDto
{
    [JsonPropertyName("id")] public string Id { get; set; } = null!;
    [JsonPropertyName("branch_id")] public string BranchId { get; set; } = null!;
    [JsonPropertyName("service_type_id")] public string ServiceTypeId { get; set; } = null!;
    [JsonPropertyName("staff_id")] public string StaffId { get; set; } = null!;
    [JsonPropertyName("start_at")] public DateTimeOffset StartAt { get; set; }
    [JsonPropertyName("end_at")] public DateTimeOffset EndAt { get; set; }
    [JsonPropertyName("capacity")] public int Capacity { get; set; }
    [JsonPropertyName("is_active")] public bool IsActive { get; set; }
}