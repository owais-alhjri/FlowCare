using System.Security.Claims;
using FlowCare.Application.Features.StaffServiceType.DTOs;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/service")]
    [ApiController]
    public class ServicesController(ServiceTypeService serviceTypeService, StaffServiceService staffServiceService)
        : ControllerBase
    {
        [HttpGet("branches/{branchId}")]
        [AllowAnonymous]
        public async Task<ActionResult> FetchServiceByBranch(string branchId)
        {
            var result = await serviceTypeService.FetchServiceByBranch(branchId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

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