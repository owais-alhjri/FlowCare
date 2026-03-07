using FlowCare.Application.Features.Slot.DTOs;
using FlowCare.Application.Interfaces.Persistence;

namespace FlowCare.Application.Interfaces.Services;

public class SlotService(ISlotsRepository slotsRepository)
{
    public async Task<List<FetchSlotDto>> FetchSlotByBranchAndServiceType(string branchId, string serviceTypeId, DateTime? date)
    {
        var filteredSlots = await slotsRepository.SlotByBranchAndServiceType(branchId, serviceTypeId, date);

        return filteredSlots.Select(s =>  new FetchSlotDto
        {
            Id = s.Id,
            BranchId = s.BranchId,
            Capacity = s.Capacity,
            EndAt = s.EndAt,
            IsActive = s.IsActive,
            ServiceTypeId = s.ServiceTypeId,
            StaffId = s.StaffId,
            StartedAt = s.StartedAt,
        }).ToList();
    }
}