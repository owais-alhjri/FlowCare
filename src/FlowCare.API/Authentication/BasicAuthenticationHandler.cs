using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using FlowCare.Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FlowCare.API.Authentication;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ICustomerService userService
    ) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.NoResult();
        }

        try
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                return AuthenticateResult.Fail("Missing Authorization header");
            }

            var header = AuthenticationHeaderValue.Parse(authHeader);
            if (header.Scheme != "Basic")
            {
                return AuthenticateResult.Fail("Invalid Scheme");
            }

            if (header.Parameter is null)
            {
                return AuthenticateResult.Fail("Missing credentials");
            }
            var credentialBytes = Convert.FromBase64String(header.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

            var identifier = credentials[0];
            var password = credentials[1];

            var user = await userService.Login(identifier, password);
            if (user is null)
            {
                return AuthenticateResult.Fail("Invalid email or password");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principle = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principle, Scheme.Name);

            return AuthenticateResult.Success(ticket);

        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
    }
}