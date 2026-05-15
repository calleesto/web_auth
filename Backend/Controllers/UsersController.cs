using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService;

namespace Backend.Controllers;

[Route("api/")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly InMemoryDatabase inMemoryDatabase;
    private readonly LoggedUsers _loggedUsers;

    public UsersController(TokenService tokenService, InMemoryDatabase inMemoryDatabase, LoggedUsers loggedUsers)
    {
        _tokenService = tokenService;
        this.inMemoryDatabase = inMemoryDatabase;
        _loggedUsers = loggedUsers;
    }
    
    // GET: api/public
    [HttpGet("public")]
    [AllowAnonymous]
    public IEnumerable<string> Get()
    {
        return ["Public value 1", "Public value 2"];
    }

    // GET api/user/1
    [HttpGet("user/{id:int}")]
    [Authorize(Policy = "UserOrAdmin")]
    public IActionResult Get(int id)
    {
        User user = inMemoryDatabase.GetUserById(id);
        UserDto userDto = new(user);
        return Ok(userDto);
    }

    // DELETE api/user/5
    [HttpDelete("user/{id}")]
    [Authorize(Roles = "admin")]
    public IActionResult Delete(int id)
    {
        User user = inMemoryDatabase.GetUserById(id);
        inMemoryDatabase.DeleteUser(user);
        return NoContent();
    }

    [HttpPost("logs")]
    [Authorize(Policy = "AdminWorkingHours")]
    public IActionResult Logs()
    {
        return Ok(_loggedUsers.GetStatus());
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        User? user = inMemoryDatabase.GetUserLogin(request.Username, request.Password);
        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }
        
        string token = _tokenService.GenerateToken(user);
        
        _loggedUsers.RegisterLogin(user);

        return Ok(new { token });
    }
    
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        _loggedUsers.RegisterLogout(int.Parse(userId));
        
        return Ok();
    }
}