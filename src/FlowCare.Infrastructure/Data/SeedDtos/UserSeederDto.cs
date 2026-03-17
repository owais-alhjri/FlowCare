using System.Text.Json.Serialization;
using FlowCare.Domain.Enums;

namespace FlowCare.Infrastructure.Data.SeedDtos;

public class UserSeederDto
{
    [JsonPropertyName("id")] public string Id { get; set; } = null!;
    [JsonPropertyName("username")] public string UserName { get; set; } = null!;
    [JsonPropertyName("password")] public string Password { get; set; } = null!;
    [JsonPropertyName("role")] public UserRole UserRole { get; set; }
    [JsonPropertyName("full_name")] public string FullName { get; set; } = null!;
    [JsonPropertyName("email")] public string Email { get; set; } = null!;
    [JsonPropertyName("phone")] public string? Phone { get; set; }
    [JsonPropertyName("branch_id")] public string? BranchId { get; set; }
    [JsonPropertyName("is_active")] public bool IsActive { get; set; }
}