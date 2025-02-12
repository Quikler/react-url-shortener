using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DAL.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WebApi.Configurations;

namespace WebApi.Providers;
public class TokenProvider(IOptions<JwtConfiguration> jwtConfiguration)
{
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    public string CreateToken(UserEntity user)
    {
        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtConfiguration.ValidIssuer,
            Audience = _jwtConfiguration.ValidAudience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_jwtConfiguration.TokenLifetime),
            SigningCredentials = creds
        };

        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }

    public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
}