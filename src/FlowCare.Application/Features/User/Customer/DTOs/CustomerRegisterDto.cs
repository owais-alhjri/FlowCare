using System.ComponentModel.DataAnnotations;
using FlowCare.Domain.Enums;

namespace FlowCare.Application.Features.User.Customer.DTOs;

public class CustomerRegisterDto
{
    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters.")]
    public string UserName { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
    public string Password { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters.")]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(12)]
    public string? Phone { get; set; }
}