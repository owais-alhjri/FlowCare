using Microsoft.AspNetCore.Http;

namespace FlowCare.Application.Features.Appointment.DTOs;

public class BookAppointmentDto
{
    public string SlotId { get; set; } = null!;
    public IFormFile? AttachmentPath { get; set; } = null!;
}