using FlowCare.Application.Common;
using FlowCare.Application.Features.User.staff.DTOs;
using FlowCare.Application.Interfaces;

namespace FlowCare.Application.Services;

public class StaffService(IStaffRepository staffRepository)
{
    public async Task<Result<List<StaffResponseDto>>> FetchStaffList(string userId)
    {
        var listOfStaff = await staffRepository.GetStaff(userId);
        if (listOfStaff is null)
            return Result<List<StaffResponseDto>>.Fail("Staff list not available");

        return Result<List<StaffResponseDto>>.Success(listOfStaff.Select(s => new StaffResponseDto
        {
            Id = s.Id,
            UserName = s.UserName,
            FullName = s.FullName,
            Email = s.Email,
            BranchId = s.BranchId,
            UserRole = s.UserRole,
            IsActive = s.IsActive
        }).ToList());
    }
}