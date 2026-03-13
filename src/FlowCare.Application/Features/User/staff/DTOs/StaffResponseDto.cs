using FlowCare.Domain.Enums;

namespace FlowCare.Application.Features.User.staff.DTOs;

public class StaffResponseDto
{
    public string Id { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public UserRole UserRole { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? BranchId { get; set; } = null!;
    public bool IsActive { get; set; }
}