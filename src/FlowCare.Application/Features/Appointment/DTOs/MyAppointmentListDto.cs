using FlowCare.Domain.Enums;

namespace FlowCare.Application.Features.Appointment.DTOs;

public class MyAppointmentListDto
{
    public string Id { get; set; } = null!;
    public string CustomerId { get;  set; } = null!;
    public string BranchId { get;  set; } = null!;
    public string ServiceTypeId { get;  set; } = null!;
    public string SlotId { get;  set; } = null!;
    public string StaffId { get;  set; } = null!;
    public Status Status { get;  set; }
    public DateTimeOffset CreatedAt { get;  set; }

}