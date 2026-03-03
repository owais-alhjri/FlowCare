namespace FlowCare.Domain.Entities;

public class Slot
{
    public string Id { get; private set; }

    //Branch ID from the entity Branch.
    public string BranchId { get; private set; }

    //Service Type ID from the entity ServiceType.
    public string ServiceTypeId { get; private set; }

    //Staff ID from entity User with the role STAFF
    public string StaffId { get; private set; }

    // DateTimeOffset used for the time zone offset
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset EndAt { get; private set; }

    // Available slots
    public int Capacity { get; private set; }
    public bool IsActive { get; private set; }

    protected Slot()
    {
    }

    public Slot(string id, string branchId, string serviceTypeId, string staffId, DateTimeOffset startedAt, int durationMinutes,
        int capacity, bool isActive)
    {
        Id = id;
        BranchId = branchId;
        ServiceTypeId = serviceTypeId;
        StaffId = staffId;
        StartedAt = DateTimeOffset.Now;

        // durationMinutes will come from the entity ServiceType, and it will be added to StartedAt then it will be stored it EntAt
        EndAt = startedAt.AddMinutes(durationMinutes);
        Capacity = capacity;
        IsActive = isActive;
    }
}