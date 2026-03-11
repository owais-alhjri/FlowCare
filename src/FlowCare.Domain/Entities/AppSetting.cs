namespace FlowCare.Domain.Entities;

public class AppSetting
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string? Description { get; set; }

    protected AppSetting()
    {
    }

    public AppSetting(string key, string value, string description)
    {
        Key = key;
        Value = value;
        Description = description;
    }

}
