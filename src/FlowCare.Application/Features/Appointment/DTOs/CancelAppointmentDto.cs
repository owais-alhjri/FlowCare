using FlowCare.Domain.Enums;

namespace FlowCare.Application.Features.Appointment.DTOs;

public class CancelAppointmentDto
{
    public string Id { get; set; } = null!;

    public Status Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

}