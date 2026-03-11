using FlowCare.Application.Features.User.Customer.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/customer")]
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
