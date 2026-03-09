using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Persistence;

public interface IStaffRepository
{
    Task<List<User>> FetchStaff(string userId);
}