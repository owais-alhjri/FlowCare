using System.ComponentModel.DataAnnotations;
using FlowCare.Domain.Enums;

namespace FlowCare.Application.Features.User.Customer.DTOs;

public class CustomerRegisterDto
{

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string UserName { get;  set; } = null!;

    [Required]
    [StringLength(100,MinimumLength = 6)]
    public string Password { get;  set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string FullName { get;  set; } = null!;


    [Required]
    [EmailAddress]
    public string Email { get;  set; } = null!;

    
    public string? Phone { get;  set; } = null!;


}