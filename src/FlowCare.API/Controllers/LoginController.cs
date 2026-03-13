using FlowCare.Application.Features.User.DTOs;
using FlowCare.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/login")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController(ICustomerService userService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> UserLogin([FromBody] UserLoginDto userLoginDto)
        {
            var result = await userService.Login(userLoginDto.Username, userLoginDto.Password);
            if (result.IsFailure)
                return Unauthorized(result.Error);

            return Ok(result.Value);
        }
    }
}