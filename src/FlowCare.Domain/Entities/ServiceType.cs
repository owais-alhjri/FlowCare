namespace FlowCare.Domain.Entities;

public class ServiceType
{
    public string Id { get; private set; }

    //Branch ID from the entity Branch.
    public string BranchId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int DurationMinutes { get; private set; }
    public bool IsActive { get; private set; }


    protected ServiceType()
    {
    }

    public ServiceType(string id, string branchId, string name, string description, int durationMinutes, bool isActive)
    {
        ValidateCommon(id, branchId, name, description, durationMinutes);
        Id = id;
        BranchId = branchId;
        Name = name;
        Description = description;
        DurationMinutes = durationMinutes;
        IsActive = isActive;
    }

    private static void ValidateCommon(string id, string branchId, string name, string description, int durationMinutes)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Length < 6 || id.Length > 100)
        {
            throw new ArgumentException("Invalid ID");
        }

        if (string.IsNullOrWhiteSpace(branchId) || branchId.Length < 6 || branchId.Length > 100)
        {
            throw new ArgumentException("Invalid branch ID");
        }

        if (string.IsNullOrWhiteSpace(name) || name.Length < 6 || name.Length > 100)
        {
            throw new ArgumentException("Invalid name");
        }

        if (string.IsNullOrWhiteSpace(description) || description.Length < 6 || description.Length > 500)
        {
            throw new ArgumentException("Invalid description");
            // max is 480 minutes = 8 hours 
        }

        if (durationMinutes < 1 || durationMinutes > 480)
        {
            throw new ArgumentException("Invalid duration minutes");
        }
    }
}