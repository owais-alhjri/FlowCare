using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces;

public interface IBranchesRepository
{
    Task<List<Branch>> BranchesList();
    Task<Branch?> FindById(string branchId);
}