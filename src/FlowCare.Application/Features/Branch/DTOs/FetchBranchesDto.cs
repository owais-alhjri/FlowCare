namespace FlowCare.Application.Features.Branch.DTOs;

public class FetchBranchesDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Timezone { get; set; } = null!;
    public bool IsActive { get; set; }
}