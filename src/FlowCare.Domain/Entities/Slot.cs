using FlowCare.Domain.Enums;

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

    public User Staff { get; private set; }

    public Slot(string id, string branchId, string serviceTypeId, User staff, DateTimeOffset startedAt, int durationMinutes,
        int capacity, bool isActive)
    {
        ValidateCommon(id, staff, branchId, serviceTypeId, durationMinutes, capacity, startedAt);
        Id = id;
        BranchId = branchId;
        ServiceTypeId = serviceTypeId;
        StaffId = staff.Id;
        StartedAt = startedAt;
        Staff = staff;

        // durationMinutes will come from the entity ServiceType, and it will be added to StartedAt then it will be stored it EntAt
        EndAt = startedAt.AddMinutes(durationMinutes);
        Capacity = capacity;
        IsActive = isActive;
    }

    private static void ValidateCommon(string id,User staff, string branchId, string serviceTypeId,
         int durationMinutes, int capacity, DateTimeOffset startedAt)
    {
        if (staff.UserRole != UserRole.STAFF)
        {
            throw new ArgumentException("User is not a staff");
        }
        if (string.IsNullOrWhiteSpace(id) || id.Length < 6 || id.Length > 100)
        {
            throw new ArgumentException("Invalid ID");
        }
        if (string.IsNullOrWhiteSpace(branchId) || branchId.Length < 6 || branchId.Length > 100)
        {
            throw new ArgumentException("Invalid branch ID");
        }
        if (string.IsNullOrWhiteSpace(serviceTypeId) || serviceTypeId.Length < 6 || serviceTypeId.Length > 100)
        {
            throw new ArgumentException("Invalid service type ID");
        }

        if (durationMinutes < 1 || durationMinutes > 480)
        {
            throw new ArgumentException("Invalid duration minutes");
        }

        if (capacity < 1 ||capacity >100)
        {
            throw new ArgumentException("Invalid capacity");
        }

        if (startedAt < DateTimeOffset.UtcNow)
        {
            throw new ArgumentException("Time must be in the future");
        }
    }

    public  void ChangeActive(bool isActive)
    {
        if (isActive == true)
        {
            IsActive = false;
        }
        else
        {
            IsActive = true;
        }
    }

    public void UpdateSlot(string? staffId, string? serviceTypeId, string? branchId,
        int? capacity, DateTimeOffset? startedAt, bool? isActive)
    {
        if (staffId is not null) StaffId = staffId;
        if (serviceTypeId is not null) ServiceTypeId = serviceTypeId;
        if (branchId is not null) BranchId = branchId;
        if (capacity is not null) Capacity = capacity.Value;
        if (startedAt is not null) StartedAt = startedAt.Value;
        if (isActive is not null) IsActive = isActive.Value;
    }
}