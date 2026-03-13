using System.ComponentModel.DataAnnotations;

namespace FlowCare.Application.Features.User.DTOs;

public class UserLoginDto
{
    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Invalid username or password")]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Invalid username or password")]
    public string Password { get; set; } = null!;
}