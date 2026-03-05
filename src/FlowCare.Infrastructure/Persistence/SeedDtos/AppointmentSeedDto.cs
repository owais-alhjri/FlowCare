using System.Text.Json.Serialization;
using FlowCare.Domain.Enums;

namespace FlowCare.Infrastructure.Persistence.SeedDtos;

public class AppointmentSeedDto
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("customer_id")] public string CustomerId { get; set; }
    [JsonPropertyName("branch_id")] public string BranchId { get; set; }
    [JsonPropertyName("service_type_id")] public string ServiceTypeId { get; set; }
    [JsonPropertyName("slot_id")] public string SlotId { get; set; }
    [JsonPropertyName("staff_id")] public string StaffId { get; set; }
    [JsonPropertyName("status")] public Status Status { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; set; }
}