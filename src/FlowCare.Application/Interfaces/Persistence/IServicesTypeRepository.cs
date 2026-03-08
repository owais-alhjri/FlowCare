using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Persistence;

public interface IServicesTypeRepository
{
    Task<List<ServiceType>> ServicesByBranch(string branchId);
    Task<ServiceType?> ExistIdAsync(string id);
}