using System.Security.Claims;
using FlowCare.Application.Features.Slot.DTOs;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/slot")]
    [ApiController]
    public class SlotsController(SlotService slotService) : ControllerBase
    {
        [HttpGet("{branchId}/services/{serviceTypeId}")]
        [AllowAnonymous]
        public async Task<ActionResult> FetchSlots(string branchId, string serviceTypeId, [FromQuery] DateTime? date)
        {
            var slots = await slotService.FetchSlotByBranchAndServiceType(branchId, serviceTypeId, date);
            return Ok(slots);
        }

        // this API for the manager nad admin to create one or bulk of slots
        [HttpPost]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<ActionResult> CreateSlot([FromBody] List<CreateSlotDto> createSlotDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            var listOfSlot = new List<object>();
            foreach (var dto in createSlotDto)
            {
                var slot = await slotService.CreateSlot(dto, userId);
                listOfSlot.Add(slot);
            }


            return Ok(listOfSlot);
        }

        // this API to update slot by slot id
        // admin => can update all slots
        // manager => can update slots for his own branch
        [HttpPatch("{slotId}")]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<ActionResult> UpdateSlot(string slotId, [FromBody] UpdateSlotDto updateSlotDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            var slot = await slotService.UpdateSlot(slotId, userId, updateSlotDto);

            return Ok(slot);
        }

        [HttpDelete("{slotId}")]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<ActionResult> DeleteSlot(string slotId)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user == null)
            {
                return Unauthorized();
            }

            var slot = await slotService.SoftDeleteSlot(slotId, user);

            return Ok(slot);
        }


    }
}
