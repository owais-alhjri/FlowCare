using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces;

public interface IServicesTypeRepository
{
    Task<List<ServiceType>> ServicesByBranch(string branchId);
    Task<ServiceType?> FindByIdAsync(string id);
}