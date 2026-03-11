using System.Security.Claims;
using FlowCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController(SlotService slotService) : ControllerBase
    {

        [HttpDelete("slots/cleanup")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> CleanUpSlots()
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (adminId is null)
            {
                return Unauthorized();
            }
            var slotsDeleted = await slotService.CleanUpSlots(adminId);
            return Ok(new { Message = $"Successfully deleted {slotsDeleted} slot" });
        }
    }
}
