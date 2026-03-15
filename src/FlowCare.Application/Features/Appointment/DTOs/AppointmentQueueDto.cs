using FlowCare.Domain.Enums;

namespace FlowCare.Application.Features.Appointment.DTOs;

public class AppointmentQueueDto
{
    public string Id { get; set; } = null!;
    public string CustomerId { get; set; } = null!;
    public string BranchId { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public int Queue { get; set; }
}