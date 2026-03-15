using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces;

public interface IStaffRepository
{
    Task<List<User>> GetStaff(string userId);
}