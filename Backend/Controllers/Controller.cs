using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService;

namespace Backend.Controllers;

[Route("api/")]
[ApiController]
public class Controller : ControllerBase
{
    private readonly TokenService _tokenService;

    public Controller(TokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    // GET: api/<Controller>
    [HttpGet]
    [Authorize]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/<Controller>/5
    [HttpGet("{id}")]
    [Authorize(Roles = "drzewo")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<Controller>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // DELETE api/<Controller>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
    
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // todo change
        if (request.Username != "Aneta" || request.Password != "haslo")
        {
            return Unauthorized("Invalid credentials");
        }

        User user = new()
        {
            Id = 1,
            Username = request.Username,
            Password = request.Password,
            Role = "drzewo"
        };


        string token = _tokenService.GenerateToken(user);

        return Ok(new { token });
    }
}