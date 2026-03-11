using System.Text.Json;
using FlowCare.Application.Features.AuditLog.DTOs;
using FlowCare.Application.Features.Slot.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Services;

public class SlotService(AuditLogService auditLogService ,ISlotsRepository slotsRepository, IBranchesRepository branchesRepository, ICustomerRepository customerRepository, IServicesTypeRepository servicesTypeRepository)
{
    public async Task<List<FetchSlotDto>> FetchSlotByBranchAndServiceType(string branchId, string serviceTypeId, DateTime? date)
    {
        var filteredSlots = await slotsRepository.SlotByBranchAndServiceType(branchId, serviceTypeId, date)
                            ?? throw new ArgumentException("Slots not available");

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

    public async Task<ResponseSlotDto> CreateSlot(CreateSlotDto createSlotDto, string userId)
    {
        var serviceType = await servicesTypeRepository.ExistIdAsync(createSlotDto.ServiceTypeId)
                          ?? throw new ArgumentException("Service Type not available");

        var branchId = serviceType.BranchId;
        var branch = await branchesRepository.FindById(branchId) ?? throw new ArgumentException("Branch not available");
        var branchLocation = branch.City.Substring(0,3).ToLower();
        var slotIdPiss = $"slot_{branchLocation}_";
        var lastUser = await slotsRepository.FetchLastId();

        string fullId;
        if (lastUser is null)
        {
            fullId = slotIdPiss + "001";
        }
        else
        {
            var lasIdString = lastUser.Id.Substring(slotIdPiss.Length);

            var lastNumber = int.Parse(lasIdString);
            var nextNumber = lastNumber + 1;

            fullId = slotIdPiss + nextNumber.ToString("D3");
        }
        var staff = await customerRepository.ExistsByStaffId(createSlotDto.StaffId)
                    ?? throw new ArgumentException("Staff is not available");


        var slot = new Slot(fullId, serviceType.BranchId, serviceType.Id
            , staff, createSlotDto.StartedAt.ToUniversalTime(), serviceType.DurationMinutes, createSlotDto.Capacity, createSlotDto.IsActive);

        await slotsRepository.CreateSlot(slot);
        await slotsRepository.SaveChangesAsync();

        var user = await customerRepository.ExistIdAsync(userId) ?? throw new ArgumentException("Customer not found");
        var log = new CreateAuditLogDto
        {
            ActorId = user.Id,
            ActorRole = user.UserRole.ToString(),
            ActionType = "CREATE_SLOT",
            EntityType = "SLOT",
            EntityId = slot.Id,
            Metadata = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                branch_id = slot.BranchId,
                service_type_id = slot.ServiceTypeId,
                staff_id = slot.StaffId

            }))

        };
        await auditLogService.AddLog(log);

        return new ResponseSlotDto
        {
            Id = slot.Id,
            BranchId = slot.BranchId,
            ServiceTypeId = slot.ServiceTypeId,
            StaffId = slot.StaffId,
            StartedAt = slot.StartedAt,
            EndAt = slot.EndAt,
            Capacity = slot.Capacity,
            IsActive = slot.IsActive
        };
    }

    public async Task<ResponseSlotDto> UpdateSlot(string slotId,string userId, UpdateSlotDto updateSlotDto)
    {
        var slot = await slotsRepository.FetchBySlotId(slotId, userId) ?? throw new ArgumentException("Slot not found or you don't have permission to update it");
        var startedAt = updateSlotDto.StartedAt.HasValue ? updateSlotDto.StartedAt.Value.ToUniversalTime()
            : slot.StartedAt;
        slot.UpdateSlot(updateSlotDto.StaffId, updateSlotDto.ServiceTypeId,
            updateSlotDto.BranchId, updateSlotDto.Capacity, startedAt, updateSlotDto.IsActive);
        await slotsRepository.SaveChangesAsync();

        var user = await customerRepository.ExistIdAsync(userId) ?? throw new ArgumentException("Customer not found");
        var log = new CreateAuditLogDto
        {
            ActorId = user.Id,
            ActorRole = user.UserRole.ToString(),
            ActionType = "UPDATE_SLOT",
            EntityType = "SLOT",
            EntityId = slot.Id,
            Metadata = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                branch_id = slot.BranchId,
                service_type_id = slot.ServiceTypeId,
                staff_id = slot.StaffId

            }))

        };
        await auditLogService.AddLog(log);

        return new ResponseSlotDto
        {
            Id = slot.Id,
            BranchId = slot.BranchId,
            ServiceTypeId = slot.ServiceTypeId,
            StaffId = slot.StaffId,
            StartedAt = slot.StartedAt,
            EndAt = slot.EndAt,
            Capacity = slot.Capacity,
            IsActive = slot.IsActive
        };

    }

    public async Task<ResponseSlotDto> SoftDeleteSlot(string slotId, string userId)
    {
        var slot = await slotsRepository.FetchBySlotId(slotId, userId) ?? throw new ArgumentException("Slot not found");
        slot.SetDeletedAt();

        var user = await customerRepository.ExistIdAsync(userId) ?? throw new ArgumentException("Customer not found");
        var log = new CreateAuditLogDto
        {
            ActorId = user.Id,
            ActorRole = user.UserRole.ToString(),
            ActionType = "DELETE_SLOT",
            EntityType = "SLOT",
            EntityId = slot.Id,
            Metadata = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                branch_id = slot.BranchId,
                service_type_id = slot.ServiceTypeId,
                staff_id = slot.StaffId,
                note = $"Slot is deleted by {user.Id}, at {slot.Deleted_at}"

            }))

        };
        await auditLogService.AddLog(log);

        await slotsRepository.SaveChangesAsync();
        return new ResponseSlotDto
        {
            Id = slot.Id,
            BranchId = slot.BranchId,
            ServiceTypeId = slot.ServiceTypeId,
            StaffId = slot.StaffId,
            StartedAt = slot.StartedAt,
            EndAt = slot.EndAt,
            Capacity = slot.Capacity,
            IsActive = slot.IsActive,
            Deleted_at = slot.Deleted_at
        };
    }
}