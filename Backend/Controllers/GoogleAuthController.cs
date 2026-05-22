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
    private readonly InMemoryDatabase inMemoryDatabase;
    private readonly LoggedUsers _loggedUsers;
    private readonly IConfiguration _configuration;

    public GoogleAuthController(TokenService tokenService, InMemoryDatabase inMemoryDatabase, LoggedUsers loggedUsers, IConfiguration configuration)
    {
        _tokenService = tokenService;
        this.inMemoryDatabase = inMemoryDatabase;
        _loggedUsers = loggedUsers;
        _configuration = configuration;
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

        User? user = inMemoryDatabase.GetUserByEmail(email);

        if (user == null)
        {
            user = inMemoryDatabase.CreateGoogleUser(name, email);
        }

        _loggedUsers.RegisterLogin(user);

        string token = _tokenService.GenerateToken(user);

        string refreshToken = _tokenService.GenerateRefreshToken();
        
        inMemoryDatabase.RefreshTokens.Add(new RefreshToken
        {
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            Token = refreshToken,
            UserId = user.Id,
            IsRevoked = false
        });

        return Redirect($"{_configuration.GetValue<string>("ApiSettings:CorsOrigins")}/index.html?token={token}&refreshToken={refreshToken}");
    }
}
