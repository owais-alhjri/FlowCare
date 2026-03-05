using System.Text.Json.Serialization;
using FlowCare.Application.Features.User.DTOs;
using FlowCare.Infrastructure.Persistence.SeedDtos;

namespace FlowCare.Infrastructure.Persistence;

public class UserGroups
{
    [JsonPropertyName("admin")] public List<UserSeederDto> Admin { get; set; }
    [JsonPropertyName("branch_managers")] public List<UserSeederDto> BranchManagers { get; set; }
    [JsonPropertyName("staff")] public List<UserSeederDto> Staff { get; set; }
    [JsonPropertyName("customers")] public List<UserSeederDto> Customers { get; set; }
}
public class SeedData
{
    [JsonPropertyName("users")] public UserGroups Users { get; set; }
    [JsonPropertyName("branches")] public List<BranchSeedDto> Branches { get; set; }
    [JsonPropertyName("service_types")] public List<ServiceTypeSeedDto> ServiceTypes { get; set; }
    [JsonPropertyName("staff_service_types")] public List<StaffServiceTypeSeedDto> StaffServiceTypes { get; set; }
    [JsonPropertyName("slots")] public List<SlotSeedDto> Slots { get; set; }
    [JsonPropertyName("appointments")] public List<AppointmentSeedDto> Appointments { get; set; }
    [JsonPropertyName("audit_logs")] public List<AuditLogSeedDto> AuditLogs { get; set; }
}



