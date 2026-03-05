using System.Text.Json.Serialization;
using FlowCare.Domain.Enums;

namespace FlowCare.Application.Features.User.DTOs;

public class UserSeederDto
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("username")] public string UserName { get; set; }
    [JsonPropertyName("password")] public string Password { get; set; }
    [JsonPropertyName("role")] public UserRole UserRole { get; set; }
    [JsonPropertyName("full_name")] public string FullName { get; set; }
    [JsonPropertyName("email")] public string Email { get; set; }
    [JsonPropertyName("phone")] public string? Phone { get; set; }
    [JsonPropertyName("branch_id")] public string? BranchId { get; set; }
    [JsonPropertyName("is_active")] public bool IsActive { get; set; }
}