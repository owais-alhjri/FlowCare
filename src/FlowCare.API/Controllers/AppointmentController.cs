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
        [Authorize(Roles = "CUSTOMER")]
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
    }
}
