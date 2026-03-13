using FlowCare.Application.Features.Branch.DTOs;
using FlowCare.Application.Interfaces;

namespace FlowCare.Application.Services;

public class BranchesService(IBranchesRepository branchesRepository)
{
    public async Task<List<FetchBranchesDto>> FetchBranches()
    {
        var  branches= await branchesRepository.BranchesList();

        return branches.Select( b => new FetchBranchesDto
        {
            Id = b.Id,
            Name = b.Name,
            City = b.City,
            Address = b.Address,
            Timezone = b.Timezone,
            IsActive = b.IsActive
        }).ToList();
    }
}