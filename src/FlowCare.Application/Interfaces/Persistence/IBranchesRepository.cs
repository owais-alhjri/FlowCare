using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Persistence;

public interface IBranchesRepository
{
    Task<List<Branch>> BranchesList();
}