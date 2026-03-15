using System.Security.Claims;
using FlowCare.Application.Features.Appointment.DTOs;
using FlowCare.Application.Interfaces;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    /// <summary>
    /// Handles appointment-related operations for customers and staff, including booking, retrieving, updating, and
    /// canceling appointments.
    /// </summary>
    [Route("api/appointment")]
    [ApiController]
    public class AppointmentController(AppointmentService appointmentService, IStorageService storageService)
        : ControllerBase
    {

        /// <summary>
        /// Books an appointment for the authenticated customer using the specified appointment details.
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
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

            return Ok(result.Value);
        }
        /// <summary>
        /// Retrieves all appointments associated with the currently logged-in user.
        /// </summary>

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

        /// <summary>
        /// Retrieves the details of a specific appointment identified by the provided appointment ID.
        /// </summary>

        [HttpGet("{appointmentId}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<ActionResult> AppointmentDetails(string appointmentId)
        {
            var result = await appointmentService.AppointmentDetails(appointmentId);
            if (result.IsFailure)
                return NotFound(result.Error);

            return Ok(result.Value);
        }
        /// <summary>
        /// Marks an appointment as 'Cancelled'. Only the owning customer can perform this.
        /// </summary>
        [HttpPatch("{appointmentId}/cancel")]
        [Authorize(Policy = "CustomerOnly")]
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

        /// <summary>
        /// Moves an existing appointment to a new time slot.
        /// </summary>

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

        /// <summary>
        /// Updates the status of an existing appointment identified by its unique ID.
        /// </summary>
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

        /// <summary>
        /// Retrieves the attachment file associated with the specified appointment.
        /// </summary>

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

        [HttpGet("queue")]
        [Authorize(Policy = "AnyAuthenticatedUser")]
        public async Task<IActionResult> GetAppointmentQueue()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var result = await appointmentService.GetAppointmentQueue(userId);
            if (result.IsFailure)
                return NotFound(result.Error);
            return Ok(result.Value);
        }
    }
}