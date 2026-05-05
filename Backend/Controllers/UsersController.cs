using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService;

namespace Backend.Controllers;

[Route("api/")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly Database _database;

    public UsersController(TokenService tokenService, Database database, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _database = database;
    }
    
    // GET: api/public
    [HttpGet("public")]
    public IEnumerable<string> Get()
    {
        return ["Public value 1", "Public value 2"];
    }

    // GET api/user/1
    [HttpGet("user/{id:int}")]
    [Authorize(Policy = "UserOrAdmin")]
    public IActionResult Get(int id)
    {
        User user = _database.GetUserById(id);
        return Ok(user);
    }

    // DELETE api/user/5
    [HttpDelete("user/{id}")]
    [Authorize(Roles = "admin")]
    public IActionResult Delete(int id)
    {
        User user = _database.GetUserById(id);
        _database.DeleteUser(user);
        return NoContent();
    }

    [HttpPost("logs")]
    [Authorize(Policy = "AdminWorkingHours")]
    public IActionResult Logs()
    {
        return Ok();
    }
    
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        User? user = _database.GetUserLogin(request.Username, request.Password);
        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }
        
        string token = _tokenService.GenerateToken(user);

        return Ok(new { token });
    }
    
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return BadRequest("Not logged in");
        }
        return Ok();
    }
}