using System.Text.Json;
using FlowCare.Application.Features.AuditLog.DTOs;
using FlowCare.Application.Features.StaffServiceType.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Services;

public class StaffServiceService(AuditLogService auditLogService,IStaffServiceTypeRepository staffServiceTypeRepository , ICustomerRepository customerRepository)
{
    public async Task<ResponseAssignedStaffDto> AssignStaffToServiceAndBranch(AssignStaffServiceTypeDto dto, string userId)
    {
        var staff = await customerRepository.ExistsByStaffId(dto.StaffId) ?? throw new ArgumentException("Staff not found");
        var serviceType = new StaffServiceType(staff, dto.ServiceTypeId);
        var assignedStaffServiceType = await staffServiceTypeRepository.AssignStaffToServiceAndBranch(serviceType, userId)
            ?? throw new ArgumentException("Error assigning staff to service");

        await staffServiceTypeRepository.SaveChangesAsync();

        var user = await customerRepository.ExistIdAsync(userId) ?? throw new ArgumentException("Customer not found");
        var log = new CreateAuditLogDto
        {
            ActorId = user.Id,
            ActorRole = user.UserRole.ToString(),
            ActionType = "CREATE_SLOT",
            EntityType = "SLOT",
            EntityId = "NO_ID",
            Metadata = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                service_type_id = assignedStaffServiceType.ServiceTypeId,
                staff_id = assignedStaffServiceType.StaffId

            }))

        };
        await auditLogService.AddLog(log);

        return new ResponseAssignedStaffDto
        {
            ServiceTypeId = assignedStaffServiceType.ServiceTypeId,
            StaffId = assignedStaffServiceType.StaffId
        };
    }
}