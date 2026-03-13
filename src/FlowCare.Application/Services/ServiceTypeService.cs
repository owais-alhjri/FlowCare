using FlowCare.Application.Common;
using FlowCare.Application.Features.ServiceType.DTOs;
using FlowCare.Application.Interfaces;

namespace FlowCare.Application.Services;

public class ServiceTypeService(IServicesTypeRepository servicesTypeRepository)
{
    public async Task<Result<List<FetchServiceTypeDto>>> FetchServiceByBranch(string branchId)
    {
        var serviceType = await servicesTypeRepository.ServicesByBranch(branchId);

        return Result<List<FetchServiceTypeDto>>.Success(serviceType.Select(s => new FetchServiceTypeDto
        {
            Id = s.Id,
            BranchId = s.BranchId,
            Description = s.Description,
            Name = s.Name,
            IsActive = s.IsActive,
            DurationMinutes = s.DurationMinutes
        }).ToList());
    }
}