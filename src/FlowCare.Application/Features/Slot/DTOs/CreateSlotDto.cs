using System.ComponentModel.DataAnnotations;

namespace FlowCare.Application.Features.Slot.DTOs;

public class CreateSlotDto
{

    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Service Type ID must be between 3 and 100 characters.")]
    public string ServiceTypeId { get;  set; }
    [Required]
    [StringLength(100, MinimumLength =6, ErrorMessage = "Staff ID must be between 3 and 100 characters.")]
    public string StaffId { get;  set; }
    [Required]
    public int Capacity { get;  set; }
    [Required]
    public bool IsActive { get;  set; }
    [Required]
    public DateTimeOffset StartedAt { get;  set; }


}