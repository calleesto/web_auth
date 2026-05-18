using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
        _accessTokenExpiryMinutes = _config.GetValue("ApiSettings:AccessTokenExpiryMinutes", 15);
    }

    public string GenerateToken(User user)
    {
        List<Claim> claims =
        [
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[64];

        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }
}