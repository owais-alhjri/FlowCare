using FlowCare.Application.Features.User.DTOs;
using FlowCare.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    /// <summary>
    /// Handles authentication requests and credential verification to provide secure access tokens for users.
    /// </summary>
    [Route("api/login")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController(ICustomerService userService) : ControllerBase
    {
        /// <summary>
        /// Authenticates a user based on their unique username or email and password.
        /// </summary>
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