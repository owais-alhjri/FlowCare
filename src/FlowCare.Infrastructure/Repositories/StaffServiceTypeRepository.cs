using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;
using FlowCare.Infrastructure.Persistence;

namespace FlowCare.Infrastructure.Repositories;

public class StaffServiceTypeRepository(FlowCareDbContext dbContext, ICustomerRepository customerRepository, IServicesTypeRepository servicesTypeRepository ) : IStaffServiceTypeRepository
{
    public async Task<StaffServiceType?> AssignStaffToServiceAndBranch(StaffServiceType staffServiceType, string userId)
    {
        var serviceTypeId = staffServiceType.ServiceTypeId;
        var serviceType = await servicesTypeRepository.ExistIdAsync(serviceTypeId)?? throw new ArgumentException("Service Type Not Found");
        var user = await customerRepository.ExistIdAsync(userId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User Not Found");
        }
        

        if (user.UserRole == UserRole.ADMIN)
        {
            await dbContext.StaffServiceTypes.AddAsync(staffServiceType);
        }

        else if (user.UserRole == UserRole.BRANCH_MANAGER)
        {
            if (serviceType.BranchId != user.BranchId || staffServiceType.Staff.BranchId != user.BranchId)
            {
                throw new UnauthorizedAccessException("Managers can only add service types to their own branch");
            }
            await dbContext.StaffServiceTypes.AddAsync(staffServiceType);
        }
        else
        {
            throw new UnauthorizedAccessException("Unauthorized role");
        }

        return staffServiceType;
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }

}