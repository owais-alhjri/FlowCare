using System.Security.Claims;
using FlowCare.Application.Features.StaffServiceType.DTOs;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing services and staff assignments within branches.
    /// </summary>
    [ApiController]
    public class ServicesController(ServiceTypeService serviceTypeService, StaffServiceService staffServiceService)
        : ControllerBase
    {
        /// <summary>
        /// Retrieves the service details associated with the specified branch identifier.
        /// </summary>
        [HttpGet("branches/{branchId}")]
        [AllowAnonymous]
        public async Task<ActionResult> FetchServiceByBranch(string branchId)
        {
            var result = await serviceTypeService.FetchServiceByBranch(branchId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
        /// <summary>
        /// Assigns a staff member to a specified service type and branch based on the provided assignment details.
        /// Admin → system-wide
        /// Manager → branch-only
        /// </summary>
        [HttpPost("assign/staff")]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<ActionResult> AssignStaffToServiceAndBranch([FromBody] AssignStaffServiceTypeDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var result = await staffServiceService.AssignStaffToServiceAndBranch(dto, userId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}