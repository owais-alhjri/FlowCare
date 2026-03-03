namespace FlowCare.Domain.Entities;

public class StaffServiceType
{
    public string StaffId { get; private set; }
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