using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("auth")]
[ApiController]
public class GoogleAuthController : ControllerBase
{
    [HttpGet("login-google")]
    public IActionResult LoginGoogle()
    {
        AuthenticationProperties properties = new()
        {
            RedirectUri = "/auth/google-callback"
        };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        AuthenticateResult result = await HttpContext.AuthenticateAsync();

        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        IEnumerable<Claim> claims = result.Principal!.Identities.First().Claims;

        return Ok(claims.Select(c => new
        {
            c.Type,
            c.Value
        }));
    }
}