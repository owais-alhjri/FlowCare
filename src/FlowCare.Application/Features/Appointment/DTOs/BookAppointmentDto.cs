using System.ComponentModel.DataAnnotations;

namespace FlowCare.Application.Features.Appointment.DTOs;

public class BookAppointmentDto
{
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string BranchId { get; set; } = null!;

    [Required]
    [StringLength(100,MinimumLength = 6)]
    public string ServiceTypeId { get; set; } = null!;
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string SlotId { get; set; } = null!;
}