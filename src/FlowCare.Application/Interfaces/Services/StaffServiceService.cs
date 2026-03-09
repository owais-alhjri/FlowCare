using FlowCare.Application.Features.StaffServiceType.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Services;

public class StaffServiceService(IStaffServiceTypeRepository staffServiceTypeRepository , ICustomerRepository customerRepository)
{
    public async Task<ResponseAssignedStaffDto> AssignStaffToServiceAndBranch(AssignStaffServiceTypeDto dto, string userId)
    {
        var staff = await customerRepository.ExistsByStaffId(dto.StaffId);
        var serviceType = new StaffServiceType(staff, dto.ServiceTypeId);
        var assignedStaffServiceType = await staffServiceTypeRepository.AssignStaffToServiceAndBranch(serviceType, userId)
            ?? throw new ArgumentException("Error assigning staff to service");

        await staffServiceTypeRepository.SaveChangesAsync();
        return new ResponseAssignedStaffDto
        {
            ServiceTypeId = assignedStaffServiceType.ServiceTypeId,
            StaffId = assignedStaffServiceType.StaffId
        };
    }
}