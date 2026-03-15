using System.Security.Claims;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing staff data, including retrieving staff members associated with the
    /// authenticated user.
    /// </summary>
    [Route("api/staff")]
    [ApiController]
    public class StaffController(StaffService staffService) : ControllerBase
    {
        /// <summary>
        /// Retrieves the list of staff members.
        /// Admin → all
        /// Manager → branch-only
        /// </summary>
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