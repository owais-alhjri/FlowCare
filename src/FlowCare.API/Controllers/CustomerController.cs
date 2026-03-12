using FlowCare.Application.Features.User.Customer.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{



    [Route("api/customer")]
    [ApiController]
    public class CustomerController(ICustomerService customerService) : ControllerBase
    {

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> CustomerRegister([FromForm] CustomerRegisterDto dto, [FromServices] FileValidationService validator)
        {
            var (isValid, error) = validator.ValidateIdImage(dto.IdImage);
            if (!isValid)
                return BadRequest(error);

            try
            {
                var newCustomer = await customerService.Register(dto);
                return Ok(newCustomer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<ActionResult> CustomerList()
        {
            var customers = await customerService.CustomerList();
            return Ok(customers);
        }

        [HttpGet("{customerId}")]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<ActionResult> GetCustomerById(string customerId)
        {
            var customers = await customerService.GetCustomerById(customerId);
            return Ok(customers);
        }

    }
}
