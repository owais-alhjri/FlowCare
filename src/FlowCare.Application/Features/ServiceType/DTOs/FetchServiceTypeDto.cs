namespace FlowCare.Application.Features.ServiceType.DTOs;

public class FetchServiceTypeDto
{
    public string Id { get; set; } = null!;
    public string BranchId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int DurationMinutes { get; set; }
    public bool IsActive { get; set; }
}