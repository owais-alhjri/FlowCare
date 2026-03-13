using System.Text.Json;
using FlowCare.Application.Common;
using FlowCare.Application.Features.AuditLog.DTOs;
using FlowCare.Application.Features.Slot.DTOs;
using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Services;

public class SlotService(
    AuditLogService auditLogService,
    ISlotsRepository slotsRepository,
    IBranchesRepository branchesRepository,
    ICustomerRepository customerRepository,
    IServicesTypeRepository servicesTypeRepository)
{
    public async Task<Result<List<FetchSlotDto>>> FetchSlotByBranchAndServiceType(string branchId, string serviceTypeId,
        DateTime? date)
    {
        var filteredSlots = await slotsRepository.SlotByBranchAndServiceType(branchId, serviceTypeId, date);
        if (filteredSlots is null)
            return Result<List<FetchSlotDto>>.Fail("Slots not available");

        return Result<List<FetchSlotDto>>.Success(filteredSlots.Select(s => new FetchSlotDto
        {
            Id = s.Id,
            BranchId = s.BranchId,
            Capacity = s.Capacity,
            EndAt = s.EndAt,
            IsActive = s.IsActive,
            ServiceTypeId = s.ServiceTypeId,
            StaffId = s.StaffId,
            StartedAt = s.StartedAt,
        }).ToList());
    }

    public async Task<Result<ResponseSlotDto>> CreateSlot(CreateSlotDto createSlotDto, string userId)
    {
        var serviceType = await servicesTypeRepository.ExistIdAsync(createSlotDto.ServiceTypeId);
        if (serviceType is null)
            return Result<ResponseSlotDto>.Fail("Service Type not available");

        var branch = await branchesRepository.FindById(serviceType.BranchId);
        if (branch is null)
            return Result<ResponseSlotDto>.Fail("Branch not available");

        var branchLocation = branch.City.Substring(0, 3).ToLower();
        var slotIdPrefix = $"slot_{branchLocation}_";
        var lastSlot = await slotsRepository.FetchLastId();

        string fullId;
        if (lastSlot is null)
            fullId = slotIdPrefix + "001";
        else
        {
            var lastIdString = lastSlot.Id.Substring(slotIdPrefix.Length);
            var lastNumber = int.Parse(lastIdString);
            fullId = slotIdPrefix + (lastNumber + 1).ToString("D3");
        }

        var staff = await customerRepository.ExistsByStaffId(createSlotDto.StaffId);
        if (staff is null)
            return Result<ResponseSlotDto>.Fail("Staff is not available");

        var slot = new Slot(fullId, serviceType.BranchId, serviceType.Id,
            staff, createSlotDto.StartedAt.ToUniversalTime(), serviceType.DurationMinutes, createSlotDto.Capacity,
            createSlotDto.IsActive);

        await slotsRepository.CreateSlot(slot);
        await slotsRepository.SaveChangesAsync();

        var user = await customerRepository.ExistIdAsync(userId);
        if (user is null)
            return Result<ResponseSlotDto>.Fail("User not found");

        var logResult = await auditLogService.AddLog(new CreateAuditLogDto
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
        });

        if (logResult.IsFailure)
            return Result<ResponseSlotDto>.Fail($"Slot created but audit log failed: {logResult.Error}");

        return Result<ResponseSlotDto>.Success(new ResponseSlotDto
        {
            Id = slot.Id,
            BranchId = slot.BranchId,
            ServiceTypeId = slot.ServiceTypeId,
            StaffId = slot.StaffId,
            StartedAt = slot.StartedAt,
            EndAt = slot.EndAt,
            Capacity = slot.Capacity,
            IsActive = slot.IsActive
        });
    }

    public async Task<Result<ResponseSlotDto>> UpdateSlot(string slotId, string userId, UpdateSlotDto updateSlotDto)
    {
        var slot = await slotsRepository.FetchBySlotId(slotId, userId);
        if (slot is null)
            return Result<ResponseSlotDto>.Fail("Slot not found or you don't have permission to update it");

        var startedAt = updateSlotDto.StartedAt.HasValue
            ? updateSlotDto.StartedAt.Value.ToUniversalTime()
            : slot.StartedAt;

        slot.UpdateSlot(updateSlotDto.StaffId, updateSlotDto.ServiceTypeId,
            updateSlotDto.BranchId, updateSlotDto.Capacity, startedAt, updateSlotDto.IsActive);
        await slotsRepository.SaveChangesAsync();

        var user = await customerRepository.ExistIdAsync(userId);
        if (user is null)
            return Result<ResponseSlotDto>.Fail("User not found");

        var logResult = await auditLogService.AddLog(new CreateAuditLogDto
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
        });

        if (logResult.IsFailure)
            return Result<ResponseSlotDto>.Fail($"Slot updated but audit log failed: {logResult.Error}");

        return Result<ResponseSlotDto>.Success(new ResponseSlotDto
        {
            Id = slot.Id,
            BranchId = slot.BranchId,
            ServiceTypeId = slot.ServiceTypeId,
            StaffId = slot.StaffId,
            StartedAt = slot.StartedAt,
            EndAt = slot.EndAt,
            Capacity = slot.Capacity,
            IsActive = slot.IsActive
        });
    }

    public async Task<Result<ResponseSlotDto>> SoftDeleteSlot(string slotId, string userId)
    {
        var slot = await slotsRepository.FetchBySlotId(slotId, userId);
        if (slot is null)
            return Result<ResponseSlotDto>.Fail("Slot not found");

        var user = await customerRepository.ExistIdAsync(userId);
        if (user is null)
            return Result<ResponseSlotDto>.Fail("User not found");

        slot.SetDeletedAt();

        var logResult = await auditLogService.AddLog(new CreateAuditLogDto
        {
            ActorId = user.Id,
            ActorRole = user.UserRole.ToString(),
            ActionType = "SOFT_DELETE_SLOT",
            EntityType = "SLOT",
            EntityId = slot.Id,
            Metadata = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                branch_id = slot.BranchId,
                service_type_id = slot.ServiceTypeId,
                staff_id = slot.StaffId,
                note = $"Slot is deleted by {user.Id}, at {slot.DeletedAt}"
            }))
        });

        if (logResult.IsFailure)
            return Result<ResponseSlotDto>.Fail($"Slot deleted but audit log failed: {logResult.Error}");

        await slotsRepository.SaveChangesAsync();

        return Result<ResponseSlotDto>.Success(new ResponseSlotDto
        {
            Id = slot.Id,
            BranchId = slot.BranchId,
            ServiceTypeId = slot.ServiceTypeId,
            StaffId = slot.StaffId,
            StartedAt = slot.StartedAt,
            EndAt = slot.EndAt,
            Capacity = slot.Capacity,
            IsActive = slot.IsActive,
            Deleted_at = slot.DeletedAt
        });
    }

    public async Task<Result<int>> CleanUpSlots(string userId)
    {
        var user = await customerRepository.ExistIdAsync(userId);
        if (user is null)
            return Result<int>.Fail("User not found");

        var slots = await slotsRepository.SlotsByDeletedAt();
        if (!slots.Any())
            return Result<int>.Success(0);

        foreach (var slot in slots)
        {
            var logResult = await auditLogService.AddLog(new CreateAuditLogDto
            {
                ActorId = user.Id,
                ActorRole = user.UserRole.ToString(),
                ActionType = "HARD_DELETE_SLOT",
                EntityType = "SLOT",
                EntityId = slot.Id,
                Metadata = JsonDocument.Parse(JsonSerializer.Serialize(new
                {
                    note = $"Slot is hard deleted by {user.Id}, at {slot.DeletedAt}"
                }))
            });

            if (logResult.IsFailure)
                return Result<int>.Fail($"Audit log failed for slot {slot.Id}: {logResult.Error}");

            slotsRepository.RemoveSlot(slot);
        }

        await slotsRepository.SaveChangesAsync();
        return Result<int>.Success(slots.Count);
    }
}