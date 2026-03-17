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
        ValidateCommon(key, value);
        Key = key;
        Value = value;
        Description = description;
    }

    private static void ValidateCommon(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key) || key.Length < 3 || key.Length > 100)
        {
            throw new ArgumentException("Invalid Key");
        }
        if (string.IsNullOrWhiteSpace(value) || key.Length < 3 || key.Length > 500)
        {
            throw new ArgumentException("Invalid Value");
        }

    }
}