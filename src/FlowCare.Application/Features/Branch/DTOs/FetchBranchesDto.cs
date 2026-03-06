
namespace FlowCare.Application.Features.Branch.DTOs;

public class FetchBranchesDto
{
    public string Id { get; set; }
    public string Name { get;  set; }
    public string City { get;  set; }
    public string Address { get;  set; }
    public string Timezone { get;  set; }
    public bool IsActive { get;  set; }
}