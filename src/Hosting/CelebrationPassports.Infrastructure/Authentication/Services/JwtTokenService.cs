using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using CelebrationPassports.Infrastructure.Authentication.Configuration;
using CelebrationPassports.Infrastructure.Authentication.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CelebrationPassports.Infrastructure.Authentication.Services;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateAccessToken(Guid userId, string email, Guid sessionId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("session_id", sessionId.ToString())
        };

        var key = JwtSigningKeyFactory.CreateKey(_options.Key);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: GetAccessTokenExpiry(),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public DateTime GetAccessTokenExpiry() =>
        DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

    public DateTime GetRefreshTokenExpiry() =>
        DateTime.UtcNow.AddDays(_options.RefreshTokenDays);
}
