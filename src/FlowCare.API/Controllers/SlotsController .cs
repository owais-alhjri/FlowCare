using FlowCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/branches/{branchId}/services/{serviceTypeId}/slots")]
    [ApiController]
    [AllowAnonymous]
    public class SlotsController(SlotService slotService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> FetchSlots(string branchId, string serviceTypeId, [FromQuery] DateTime? date)
        {
            var slots = await slotService.FetchSlotByBranchAndServiceType(branchId, serviceTypeId, date);
            return Ok(slots);
        }

    }
}
