namespace FlowCare.Domain.Entities;

public class Branch
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string City { get; private set; }
    public string Address { get; private set; }
    public string Timezone { get; private set; }
    public bool IsActive { get; private set; }

    protected Branch()
    {
    }

    public Branch(string id, string name, string city, string address, string timezone, bool isActive)
    {
        Id = id;
        Name = name;
        City = city;
        Address = address;
        Timezone = timezone;
        IsActive = isActive;
    }

}