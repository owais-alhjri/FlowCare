using FlowCare.Domain.Enums;

namespace FlowCare.Domain.Entities;

public class StaffServiceType
{
    //Staff ID from entity User with the role STAFF
    public string StaffId { get; private set; }

    //Service Type ID from the entity ServiceType.
    public string ServiceTypeId { get; private set; }

    protected StaffServiceType()
    {
    }

    public User Staff { get; private set; }

    public StaffServiceType(User staff , string serviceTypeId)
    {
        ValidateCommon(staff, serviceTypeId);
        StaffId = staff.Id;
        ServiceTypeId = serviceTypeId;
        Staff = staff;
    }

    private static void ValidateCommon(User staff, string serviceTypeId)
    {
        if (staff.UserRole != UserRole.STAFF)
        {
            throw new ArgumentException("User is not a staff");
        }
        if (string.IsNullOrWhiteSpace(staff.Id) || staff.Id.Length < 3 || staff.Id.Length >100)
        {
            throw new ArgumentException("Invalid staff ID");
        }

        if (string.IsNullOrWhiteSpace(serviceTypeId) || serviceTypeId.Length <3 || serviceTypeId.Length > 100)
        {
            throw new ArgumentException("Invalid Service type ID");
        }
    }
}