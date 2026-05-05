using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserService;

namespace Backend;

public class TokenService
{
    private readonly IConfiguration _config;
    private readonly string _secretKey;
    private readonly int _accessTokenExpiryMinutes;

    public TokenService(IConfiguration config)
    {
        _config = config;
        _secretKey = _config["ApiSettings:Secret"];
        _accessTokenExpiryMinutes = _config.GetValue("ApiSettings:AccessTokenExpiryMinutes", 60);
    }

    public string GenerateToken(User user)
    {
        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_secretKey));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: _config["ApiSettings:Issuer"],
            audience: _config["ApiSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(_accessTokenExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}