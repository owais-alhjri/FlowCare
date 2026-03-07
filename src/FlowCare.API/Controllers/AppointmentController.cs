using System.Security.Claims;
using FlowCare.Application.Features.Appointment.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController(AppointmentService appointmentService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult> BookAppointment(BookAppointmentDto bookAppointmentDto)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (customerId is null)
            {
                return Unauthorized();
            }
            await appointmentService.BookAppointment(bookAppointmentDto, customerId);

            return Ok();
        }

        [HttpGet]
        [Authorize(Policy = "AnyAuthenticatedUser")]
        public async Task<ActionResult> AppointmentById()
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (customerId is null)
            {
                return Unauthorized();
            }

            var appointmentById = await appointmentService.AppointmentById(customerId);

            return Ok(appointmentById);
        }

        [HttpGet("{appointmentId}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult> AppointmentDetails(string appointmentId)
        {
            var appointment = await appointmentService.AppointmentDetails(appointmentId);

            return Ok(appointment);
        }

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
            var appointment = await appointmentService.CancelAppointment(appointmentId);


            return Ok(appointment);
        }

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
            var appointment = await appointmentService.Reschedule(appointmentId, slotId);

            return Ok(appointment);
        }
    }
}
