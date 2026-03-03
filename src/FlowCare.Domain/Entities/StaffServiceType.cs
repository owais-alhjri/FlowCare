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

    public StaffServiceType(string staffId, string serviceTypeId)
    {
        StaffId = staffId;
        ServiceTypeId = serviceTypeId;
    }
}