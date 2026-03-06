using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Persistence;

public interface ISlotsRepository
{
    Task<List<Slot>> SlotByBranchAndServiceType(string branchId, string serviceTypeId, DateTime? date);
}