using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Persistence;

public interface IStaffServiceTypeRepository
{
    Task<StaffServiceType?> AssignStaffToServiceAndBranch(StaffServiceType staffServiceType, string userId);
    Task SaveChangesAsync();
}