namespace FlowCare.Application.Features.Slot.DTOs;

public class FetchSlotDto
{
    public string Id { get;  set; } = null!;
    public string BranchId { get;  set; } = null!;
    public string ServiceTypeId { get;  set; } = null!;
    public string StaffId { get;  set; } = null!;
    public DateTimeOffset StartedAt { get;  set; }
    public DateTimeOffset EndAt { get;  set; }
    public int Capacity { get;  set; }
    public bool IsActive { get;  set; }

}