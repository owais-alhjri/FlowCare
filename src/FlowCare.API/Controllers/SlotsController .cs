using System.Security.Claims;
using FlowCare.Application.Features.Slot.DTOs;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing slots, including retrieving, creating, updating, and deleting slots for
    /// specific branches and service types.
    /// </summary>
    [Route("api/slot")]
    [ApiController]
    public class SlotsController(SlotService slotService) : ControllerBase
    {
        /// <summary>
        /// Retrieves available appointment slots filtered by branch, service type, and an optional date.
        /// </summary>
        [HttpGet("{branchId}/services/{serviceTypeId}")]
        [AllowAnonymous]
        public async Task<ActionResult> FetchSlots(string branchId, string serviceTypeId, [FromQuery] DateTime? date)
        {
            var result = await slotService.FetchSlotByBranchAndServiceType(branchId, serviceTypeId, date);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
        /// <summary>
        /// Create slots for a branch (single or bulk).
        /// </summary>

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
        /// <summary>
        /// Updates the details of an existing slot with the specified information.
        /// </summary>
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
        /// <summary>
        /// Performs a soft delete on a specific slot, removing it from public view while preserving historical data for auditing.
        /// </summary>
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