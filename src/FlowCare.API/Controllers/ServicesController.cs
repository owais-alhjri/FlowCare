using System.Security.Claims;
using FlowCare.Application.Features.StaffServiceType.DTOs;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/service")]
    [ApiController]
    public class ServicesController(ServiceTypeService serviceTypeService, StaffServiceService staffServiceService) : ControllerBase
    {
        [HttpGet("api/branches/{branchId}/services")]
        [AllowAnonymous]
        public async Task<ActionResult> FetchServiceByBranch(string branchId)
        {
            var services = await serviceTypeService.FetchServiceByBranch(branchId);
            return Ok(services);
        }

        [HttpPost("assign/staff")]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<ActionResult> AssignStaffToServiceAndBranch([FromBody] AssignStaffServiceTypeDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();
            var assignedStaff = await staffServiceService.AssignStaffToServiceAndBranch(dto, userId);

            return Ok(assignedStaff);

        }

    }
}
