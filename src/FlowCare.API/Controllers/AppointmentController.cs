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
    [Authorize(Roles = "CUSTOMER")]
    public class AppointmentController(AppointmentService appointmentService) : ControllerBase
    {
        [HttpPost]
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
    }
}
