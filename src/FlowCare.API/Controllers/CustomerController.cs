using FlowCare.Application.Features.User.Customer.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(ICustomerService customerService) : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CustomerRegister([FromBody] CustomerRegisterDto customerRegisterDto)
        {
            var newCustomer = await customerService.Register(customerRegisterDto);

            return Ok(newCustomer);
        }

    }
}
