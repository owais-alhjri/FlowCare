using System.ComponentModel.DataAnnotations;

namespace FlowCare.Application.Features.StaffServiceType.DTOs;

public class AssignStaffServiceTypeDto
{
    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Staff ID must be between 6 and 100 characters.")]

    public string StaffId { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Service Type ID must be between 6 and 100 characters.")]
    public string ServiceTypeId { get; set; } = null!;
}