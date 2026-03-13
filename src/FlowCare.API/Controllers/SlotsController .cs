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
            var result = await slotService.FetchSlotByBranchAndServiceType(branchId, serviceTypeId, date);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<ActionResult> CreateSlot([FromBody] List<CreateSlotDto> createSlotDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var listOfSlots = new List<ResponseSlotDto>();
            foreach (var dto in createSlotDto)
            {
                var result = await slotService.CreateSlot(dto, userId);
                if (result.IsFailure)
                    return BadRequest(result.Error);

                listOfSlots.Add(result.Value!);
            }

            return Ok(listOfSlots);
        }

        [HttpPatch("{slotId}")]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<ActionResult> UpdateSlot(string slotId, [FromBody] UpdateSlotDto updateSlotDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var result = await slotService.UpdateSlot(slotId, userId, updateSlotDto);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpDelete("{slotId}")]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<ActionResult> DeleteSlot(string slotId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var result = await slotService.SoftDeleteSlot(slotId, userId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}