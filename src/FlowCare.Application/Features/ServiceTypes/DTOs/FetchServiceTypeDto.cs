namespace FlowCare.Application.Features.ServiceType.DTOs;

public class FetchServiceTypeDto
{
    public string Id { get;  set; }
    public string BranchId { get;  set; }
    public string Name { get;  set; }
    public string Description { get;  set; }
    public int DurationMinutes { get;  set; }
    public bool IsActive { get;  set; }
}