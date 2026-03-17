using System.Text.Json.Serialization;
using FlowCare.Infrastructure.Data.SeedDtos;

namespace FlowCare.Infrastructure.Data;

public class UserGroups
{
    [JsonPropertyName("admin")] public List<UserSeederDto> Admin { get; set; } = null!;
    [JsonPropertyName("branch_managers")] public List<UserSeederDto> BranchManagers { get; set; } = null!;
    [JsonPropertyName("staff")] public List<UserSeederDto> Staff { get; set; } = null!;
    [JsonPropertyName("customers")] public List<UserSeederDto> Customers { get; set; } = null!;
}

public class SeedData
{
    [JsonPropertyName("users")] public UserGroups Users { get; set; } = null!;
    [JsonPropertyName("branches")] public List<BranchSeedDto> Branches { get; set; } = null!;
    [JsonPropertyName("service_types")] public List<ServiceTypeSeedDto> ServiceTypes { get; set; } = null!;

    [JsonPropertyName("staff_service_types")]
    public List<StaffServiceTypeSeedDto> StaffServiceTypes { get; set; } = null!;

    [JsonPropertyName("slots")] public List<SlotSeedDto> Slots { get; set; } = null!;
    [JsonPropertyName("appointments")] public List<AppointmentSeedDto> Appointments { get; set; } = null!;
    [JsonPropertyName("audit_logs")] public List<AuditLogSeedDto> AuditLogs { get; set; } = null!;
    [JsonPropertyName("system_settings")] public List<SystemSettingSeedDto>? SystemSettings { get; set; }
}