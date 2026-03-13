using System.Security.Claims;
using FlowCare.Application.Features.Appointment.DTOs;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/appointment")]
    [ApiController]
    public class AppointmentController(AppointmentService appointmentService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult> BookAppointment([FromForm] BookAppointmentDto dto, [FromServices] FileValidationService validator)
        {

            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (customerId is null)
            {
                return Unauthorized();
            }

            if (dto.AttachmentPath != null)
            {
                var (isValid, error) = validator.ValidateAttachment(dto.AttachmentPath);
                if (!isValid)
                    return BadRequest(error);
            }

            try
            {
                await appointmentService.BookAppointment(dto, customerId);
                return Ok();

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }
        // Staff → view appointments assigned to them
        // Branch Manager → view appointments within their branch
        // Admin → view appointments across all branches
        // customer → view his own appointments
        [HttpGet]
        [Authorize(Policy = "AnyAuthenticatedUser")]
        public async Task<ActionResult> AppointmentById()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            var appointmentById = await appointmentService.AppointmentById(userId);

            return Ok(appointmentById);
        }
        // this for the customer to get appointment details 
        [HttpGet("{appointmentId}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult> AppointmentDetails(string appointmentId)
        {
            var appointment = await appointmentService.AppointmentDetails(appointmentId);

            return Ok(appointment);
        }

        // this for the customer to cancel his appointment
        [HttpPatch("{appointmentId}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult> CancelAppointment(string appointmentId)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (customerId is null)
            {
                return Unauthorized();
            }
            var appointmentIdChecker = await appointmentService.AppointmentDetails(appointmentId)
                                       ?? throw new ArgumentException("Appointment no found");

            if (customerId != appointmentIdChecker.CustomerId)
            {
                return Unauthorized();
            }
            var appointment = await appointmentService.CancelAppointment(appointmentId, customerId);


            return Ok(appointment);
        }

        // this for the customer to reschedule his appointment
        [HttpPatch("{appointmentId}/reschedule")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult> RescheduleAppointment(string appointmentId, [FromBody] string slotId)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var appointmentIdChecker = await appointmentService.AppointmentDetails(appointmentId)
                                       ?? throw new ArgumentException("Appointment no found");
            if (customerId != appointmentIdChecker.CustomerId)
            {
                return Unauthorized();
            }
            var appointment = await appointmentService.Reschedule(appointmentId, slotId, customerId);

            return Ok(appointment);
        }

        // Staff → update the status appointments assigned to them
        // Branch Manager → update the status appointments within their branch
        // Admin → update the status appointments across all branches
        [HttpPatch("{appointmentId}/update/status")]
        [Authorize(Policy = "StaffOrAbove")]
        public async Task<ActionResult> UpdateAppointmentStatus(string appointmentId, [FromBody] string status )
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }



            var appointment = await appointmentService.UpdateAppointmentStatus(appointmentId, userId, status);


            return Ok(appointment);
        }
    }
}
