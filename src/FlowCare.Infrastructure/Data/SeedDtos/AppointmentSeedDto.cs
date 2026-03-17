using System.Text.Json.Serialization;
using FlowCare.Domain.Enums;

namespace FlowCare.Infrastructure.Data.SeedDtos;

public class AppointmentSeedDto
{
    [JsonPropertyName("id")] public string Id { get; set; } = null!;
    [JsonPropertyName("customer_id")] public string CustomerId { get; set; } = null!;
    [JsonPropertyName("branch_id")] public string BranchId { get; set; } = null!;
    [JsonPropertyName("service_type_id")] public string ServiceTypeId { get; set; } = null!;
    [JsonPropertyName("slot_id")] public string SlotId { get; set; } = null!;
    [JsonPropertyName("staff_id")] public string StaffId { get; set; } = null!;
    [JsonPropertyName("status")] public Status Status { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; set; }
    [JsonPropertyName("queue")] public int Queue { get; set; }

}