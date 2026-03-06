using FlowCare.Application.Features.ServiceType.DTOs;
using FlowCare.Application.Interfaces.Persistence;

namespace FlowCare.Application.Interfaces.Services;

public class ServiceTypeService(IServicesTypeRepository servicesTypeRepository)
{
    public async Task<List<FetchServiceTypeDto>> FetchServiceByBranch(string branchId)
    {
        var serviceType = await servicesTypeRepository.ServicesByBranch(branchId);

        return serviceType.Select(s => new FetchServiceTypeDto
        {
            Id = s.BranchId,
            BranchId = s.BranchId,
            Description = s.Description,
            Name = s.Name,
            IsActive = s.IsActive,
            DurationMinutes = s.DurationMinutes

        }).ToList();
    }
}