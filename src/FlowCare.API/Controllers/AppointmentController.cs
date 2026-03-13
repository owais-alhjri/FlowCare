using System.Security.Claims;
using FlowCare.Application.Features.Appointment.DTOs;
using FlowCare.Application.Interfaces;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/appointment")]
    [ApiController]
    public class AppointmentController(AppointmentService appointmentService, IStorageService storageService)
        : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult> BookAppointment([FromForm] BookAppointmentDto dto,
            [FromServices] FileValidationService validator)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (customerId is null)
                return Unauthorized();

            if (dto.AttachmentPath != null)
            {
                var (isValid, error) = validator.ValidateAttachment(dto.AttachmentPath);
                if (!isValid)
                    return BadRequest(error);
            }

            var result = await appointmentService.BookAppointment(dto, customerId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok();
        }

        [HttpGet]
        [Authorize(Policy = "AnyAuthenticatedUser")]
        public async Task<ActionResult> AppointmentById()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var result = await appointmentService.AppointmentById(userId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{appointmentId}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult> AppointmentDetails(string appointmentId)
        {
            var result = await appointmentService.AppointmentDetails(appointmentId);
            if (result.IsFailure)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpPatch("{appointmentId}/cancel")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult> CancelAppointment(string appointmentId)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (customerId is null)
                return Unauthorized();

            var result = await appointmentService.CancelAppointment(appointmentId, customerId);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPatch("{appointmentId}/reschedule")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult> RescheduleAppointment(string appointmentId, [FromBody] string slotId)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (customerId is null)
                return Unauthorized();

            var detailsResult = await appointmentService.AppointmentDetails(appointmentId);
            if (detailsResult.IsFailure)
                return NotFound(detailsResult.Error);

            if (customerId != detailsResult.Value!.CustomerId)
                return Unauthorized();

            var result = await appointmentService.Reschedule(appointmentId, slotId, customerId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPatch("{appointmentId}/update/status")]
        [Authorize(Policy = "StaffOrAbove")]
        public async Task<ActionResult> UpdateAppointmentStatus(string appointmentId, [FromBody] string status)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var result = await appointmentService.UpdateAppointmentStatus(appointmentId, userId, status);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("attachment/{appointmentId}")]
        [Authorize(Policy = "AnyAuthenticatedUser")]
        public async Task<IActionResult> GetAttachment(string appointmentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var result = await appointmentService.GetAppointmentAttachment(appointmentId, userId);
            if (result.IsFailure)
                return NotFound(result.Error);

            if (string.IsNullOrEmpty(result.Value!.AttachmentPath))
                return NotFound("No attachment found for this appointment");

            var (stream, contentType) = await storageService.GetFileAsync(result.Value.AttachmentPath);
            return File(stream, contentType);
        }
    }
}