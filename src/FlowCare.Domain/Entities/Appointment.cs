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
    public string? SlotId { get; private set; }
    
    //Staff ID from entity User with the role STAFF
    public string StaffId { get; private set; }
    public Status Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public string? AttachmentPath { get; private set; }

    protected Appointment()
    {
    }

    // these are for relationships
    public User Customer { get; private set; }
    public User Staff { get; private set; }
    public Slot? Slot { get; private set; }

    public Appointment(string id, User customer, User staff, string branchId,
        string serviceTypeId, string slotId, Status status, DateTimeOffset createdAt)
    {
        ValidateRole(customer, staff, branchId);
        ValidateCommon(id, branchId, serviceTypeId, slotId);

        Id = id;
        CustomerId = customer.Id;
        BranchId = branchId;
        ServiceTypeId = serviceTypeId;
        SlotId = slotId;
        StaffId = staff.Id;
        Status = status;
        CreatedAt = createdAt;
        Customer = customer;
        Staff = staff;
    }

    private static void ValidateRole(User customer, User staff, string branchId)
    {
        if (customer.UserRole != UserRole.CUSTOMER)
        {
            throw new ArgumentException("User is not a customer");
        }

        if (staff.UserRole != UserRole.STAFF)
        {
            throw new ArgumentException("User is not a staff");
        }

        if (staff.BranchId != branchId )
        {
            throw new ArgumentException("Staff dose not belong to this branch");
        }
    }
    private static void ValidateCommon(string id, string branchId,
        string serviceTypeId, string slotId)
    {
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
        if (string.IsNullOrWhiteSpace(slotId) || slotId.Length < 6 || slotId.Length > 100)
        {
            throw new ArgumentException("Invalid slot ID");
        }

    }

    public void CanceledAppointment()
    {
        if (Status == Status.CANCELLED)
        {
            throw new ArgumentException("Status is already canceled");
        }

        Status = Status.CANCELLED;
        CreatedAt = DateTimeOffset.UtcNow;
    }
    public void RescheduleAppointmentSlot(string currentSlotId, string newSlotId)
    {
        if (currentSlotId == newSlotId)
        {
            throw new ArgumentException("You can not change the slot to the same one");
        }

        Status = Status.RESCHEDULE;
        SlotId = newSlotId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateAppointmentStatus(string status, string newStatus)
    {
        if (status == newStatus)
        {
            throw new ArgumentException("You can not change the slot to the same one");
        }

        if (newStatus == "COMPLETED")
        {
            Status = Status.COMPLETED;
        }
        if (newStatus == "NO_SHOW")
        {
            Status = Status.NO_SHOW;
        }
        if (newStatus == "CHECKED_IN")
        {
            Status = Status.CHECKED_IN;
        }

        CreatedAt = DateTimeOffset.UtcNow;
    }

}