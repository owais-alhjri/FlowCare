using FlowCare.Application.Features.Slot.DTOs;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Persistence;

public interface ISlotsRepository
{
    Task<List<Slot>> SlotByBranchAndServiceType(string branchId, string serviceTypeId, DateTime? date);
    Task<Slot?> FindSlot(string slotId);
    Task CreateSlot(Slot slot );
    Task<Slot?> FetchLastId();
    Task SaveChangesAsync();
    Task<Slot?>  FetchBySlotId(string slotId, string userId);
    Task<List<Slot>> SlotsByDeletedAt();
    void RemoveSlot(Slot slot);
}