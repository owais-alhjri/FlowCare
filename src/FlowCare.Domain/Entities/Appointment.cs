using FlowCare.Domain.Enums;

namespace FlowCare.Domain.Entities;

public class Appointment
{
    public string Id { get; private set; }

    // Customer ID from the entity User with the role CUSTOMER.
    public string CustomerId { get; private set; }

    //Branch ID from the entity Branch.
    public string BranchId { get; private set; }

    //Service Type ID from the entity ServiceType.
    public string ServiceTypeId { get; private set; }

    //Slot ID from the entity slot.
    public string SlotId { get; private set; }
    
    //Staff ID from entity User with the role STAFF
    public string StaffId { get; private set; }
    public Status Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    protected Appointment()
    {
    }

    public Appointment(string id, string customerId, string branchId,
        string serviceTypeId, string slotId, string staffId, Status status, DateTimeOffset createdAt)
    {
        Id = id;
        CustomerId = customerId;
        BranchId = branchId;
        ServiceTypeId = serviceTypeId;
        SlotId = slotId;
        StaffId = staffId;
        Status = status;
        CreatedAt = DateTimeOffset.Now;
    }

}