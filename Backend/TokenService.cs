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
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new (ClaimTypes.Email, user.Email)
        ];

        foreach (string role in user.Roles)
        {
            Claim claim = new(ClaimTypes.Role, role);
            claims.Add(claim);
        }

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