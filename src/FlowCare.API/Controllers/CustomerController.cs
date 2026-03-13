using FlowCare.Application.Features.User.Customer.DTOs;
using FlowCare.Application.Interfaces;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController(ICustomerService customerService, IStorageService storageService) : ControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> CustomerRegister([FromForm] CustomerRegisterDto dto,
            [FromServices] FileValidationService validator)
        {
            var (isValid, error) = validator.ValidateIdImage(dto.IdImage);
            if (!isValid)
                return BadRequest(error);

            var result = await customerService.Register(dto);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<ActionResult> CustomerList()
        {
            var result = await customerService.CustomerList();
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpGet("{customerId}")]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<IActionResult> GetCustomerById(string customerId)
        {
            var result = await customerService.GetCustomerById(customerId);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpGet("{customerId}/id-image")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetIdImage(string customerId)
        {
            var result = await customerService.GetCustomerById(customerId);
            if (result.IsFailure)
                return NotFound(result.Error);

            if (string.IsNullOrEmpty(result.Value!.IdImagePath))
                return NotFound("No ID image found");

            var (stream, contentType) = await storageService.GetFileAsync(result.Value.IdImagePath);
            return File(stream, contentType);
        }
    }
}