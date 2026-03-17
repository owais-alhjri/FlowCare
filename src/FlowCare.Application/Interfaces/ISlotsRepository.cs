using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces;

public interface ISlotsRepository
{
    Task<List<Slot>> SlotByBranchAndServiceType(string branchId, string serviceTypeId, DateTime? date);
    Task<Slot?> FindSlot(string slotId);
    Task CreateSlot(Slot slot);
    Task<Slot?> GetLastId();
    Task SaveChangesAsync();
    Task<Slot?> GetBySlotId(string slotId, string userId);
    Task<List<Slot>> SlotsByDeletedAt();
    void RemoveSlot(Slot slot);
}