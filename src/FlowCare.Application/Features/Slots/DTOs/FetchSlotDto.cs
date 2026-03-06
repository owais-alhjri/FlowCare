namespace FlowCare.Application.Features.Slots.DTOs;

public class FetchSlotDto
{
    public string Id { get;  set; }
    public string BranchId { get;  set; }
    public string ServiceTypeId { get;  set; }
    public string StaffId { get;  set; }
    public DateTimeOffset StartedAt { get;  set; }
    public DateTimeOffset EndAt { get;  set; }
    public int Capacity { get;  set; }
    public bool IsActive { get;  set; }

}