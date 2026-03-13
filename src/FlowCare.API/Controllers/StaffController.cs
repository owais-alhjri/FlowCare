using System.Security.Claims;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/staff")]
    [ApiController]
    public class StaffController(StaffService staffService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<ActionResult> FetchStaff()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var result = await staffService.FetchStaffList(userId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}