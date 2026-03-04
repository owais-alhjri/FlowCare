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
        ValidateCommon(id, name, city, address, timezone);

        Id = id;
        Name = name;
        City = city;
        Address = address;
        Timezone = timezone;
        IsActive = isActive;
    }

    private static void ValidateCommon(string id, string name, string city, string address, string timezone)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Length < 3 || id.Length > 100)
        {
            throw new ArgumentException("Invalid ID");
        }
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 100)
        {
            throw new ArgumentException("Invalid name");
        }
        if (string.IsNullOrWhiteSpace(city) || city.Length < 3 || city.Length > 100)
        {
            throw new ArgumentException("Invalid city");
        }
        if (string.IsNullOrWhiteSpace(address) || address.Length < 3 || address.Length > 100)
        {
            throw new ArgumentException("Invalid address");
        }
        if (string.IsNullOrWhiteSpace(timezone) || timezone.Length < 3 || timezone.Length > 100)
        {
            throw new ArgumentException("Invalid timezone");
        }
    }

}