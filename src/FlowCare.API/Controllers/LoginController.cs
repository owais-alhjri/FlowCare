using FlowCare.Application.Features.User.DTOs;
using FlowCare.Application.Interfaces.Persistence;
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
            var username = userLoginDto.Username;
            var password = userLoginDto.Password;
            var user = await userService.Login(username, password);

            return Ok(user);
        }
    }
}
