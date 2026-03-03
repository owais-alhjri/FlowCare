namespace FlowCare.Domain.Entities;

public class Slots
{
    public string Id { get; private set; }
    public string BranchId { get; private set; }
    public string ServiceTypeId { get; private set; }
    public string StaffId { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset EndAt { get; private set; }
    public int Capacity { get; private set; }
    public bool IsActive { get; private set; }

    protected Slots()
    {
    }

    public Slots(string id, string branchId, string serviceTypeId, string staffId, DateTimeOffset startedAt, int durationMinutes,
        int capacity, bool isActive)
    {
        Id = id;
        BranchId = branchId;
        ServiceTypeId = serviceTypeId;
        StaffId = staffId;
        StartedAt = DateTimeOffset.Now;
        EndAt = startedAt.AddMinutes(durationMinutes);
        Capacity = capacity;
        IsActive = isActive;
    }
}