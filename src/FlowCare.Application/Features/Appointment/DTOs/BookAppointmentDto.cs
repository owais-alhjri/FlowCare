using System.ComponentModel.DataAnnotations;

namespace FlowCare.Application.Features.Appointment.DTOs;

public class BookAppointmentDto
{
    public string SlotId { get; set; } = null!;
}