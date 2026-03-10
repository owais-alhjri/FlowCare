using FlowCare.Application.Features.User.staff.DTOs;
using FlowCare.Application.Interfaces.Persistence;

namespace FlowCare.Application.Interfaces.Services;

public class StaffService(IStaffRepository staffRepository)
{
    public async Task<List<StaffResponseDto>> FetchStaffList(string userId)
    {
        var listOfStaff = await staffRepository.FetchStaff(userId);
        if (listOfStaff is null)
        {
            throw new ArgumentException("Staff list not available");
        }

        return listOfStaff.Select(s=> new StaffResponseDto
            {
            Id = s.Id,
            UserName = s.UserName,
            FullName = s.FullName,
            Email = s.Email,
            BranchId = s.BranchId,
            UserRole = s.UserRole,
            IsActive = s.IsActive
            }).ToList();
    }
}