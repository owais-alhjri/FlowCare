namespace FlowCare.Domain.Entities;

public class ServiceType
{
    public string Id { get; private set; }

    //Branch ID from the entity Branch.
    public string BranchId { get; private set; }
    public string Name{ get; private set; }
    public string Description{ get; private set; }
    public int DurationMinutes { get; private set; }
    public bool IsActive{ get; private set; }


    protected ServiceType()
    {
    }

    public ServiceType(string id, string branchId, string name, string description, int durationMinutes, bool isActive)
    {
        Id = id;
        BranchId = branchId;
        Name = name;
        Description = description;
        DurationMinutes = durationMinutes;
        IsActive = isActive;
    }
}