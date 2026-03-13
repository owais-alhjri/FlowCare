using System.Text.Json;
using FlowCare.Application.Common;
using FlowCare.Application.Features.AuditLog.DTOs;
using FlowCare.Application.Features.StaffServiceType.DTOs;
using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Services;

public class StaffServiceService(
    AuditLogService auditLogService,
    IStaffServiceTypeRepository staffServiceTypeRepository,
    ICustomerRepository customerRepository)
{
    public async Task<Result<ResponseAssignedStaffDto>> AssignStaffToServiceAndBranch(AssignStaffServiceTypeDto dto,
        string userId)
    {
        var staff = await customerRepository.ExistsByStaffId(dto.StaffId);
        if (staff is null)
            return Result<ResponseAssignedStaffDto>.Fail("Staff not found");

        var serviceType = new StaffServiceType(staff, dto.ServiceTypeId);
        var assignedStaffServiceType =
            await staffServiceTypeRepository.AssignStaffToServiceAndBranch(serviceType, userId);
        if (assignedStaffServiceType is null)
            return Result<ResponseAssignedStaffDto>.Fail("Error assigning staff to service");

        await staffServiceTypeRepository.SaveChangesAsync();

        var user = await customerRepository.ExistIdAsync(userId);
        if (user is null)
            return Result<ResponseAssignedStaffDto>.Fail("User not found");

        var logResult = await auditLogService.AddLog(new CreateAuditLogDto
        {
            ActorId = user.Id,
            ActorRole = user.UserRole.ToString(),
            ActionType = "ASSIGN_STAFF_SERVICE",
            EntityType = "STAFF_SERVICE_TYPE",
            EntityId = assignedStaffServiceType.StaffId,
            Metadata = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                service_type_id = assignedStaffServiceType.ServiceTypeId,
                staff_id = assignedStaffServiceType.StaffId
            }))
        });

        if (logResult.IsFailure)
            return Result<ResponseAssignedStaffDto>.Fail($"Staff assigned but audit log failed: {logResult.Error}");

        return Result<ResponseAssignedStaffDto>.Success(new ResponseAssignedStaffDto
        {
            ServiceTypeId = assignedStaffServiceType.ServiceTypeId,
            StaffId = assignedStaffServiceType.StaffId
        });
    }
}