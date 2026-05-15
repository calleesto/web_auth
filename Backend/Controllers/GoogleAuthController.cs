using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using UserService;

namespace Backend.Controllers;

[Route("auth")]
[ApiController]
public class GoogleAuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly Database _database;
    private readonly LoggedUsers _loggedUsers;
    
    public GoogleAuthController(TokenService tokenService, Database database, LoggedUsers loggedUsers)
    {
        _tokenService = tokenService;
        _database = database;
        _loggedUsers = loggedUsers;
    }
    
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
        AuthenticateResult result = await HttpContext.AuthenticateAsync("External");

        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        ClaimsPrincipal principal = result.Principal!;

        string email = principal.FindFirst(ClaimTypes.Email)!.Value;
        string name = principal.FindFirst(ClaimTypes.Name)!.Value;

        User? user = _database.GetUserByEmail(email);

        if (user == null)
        {
            user = _database.CreateGoogleUser(name, email);
            _loggedUsers.RegisterLogin(user);
        }

        string jwt = _tokenService.GenerateToken(user);

        return Ok(new
        {
            token = jwt
        });
    }
}