using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CelebrationPassports.Infrastructure.Authentication;

// Shared between token generation (JwtTokenService) and token validation (API's
// AddJwtBearer setup) so both derive the exact same signing key from the configured
// Jwt:Key — a plain dev secret, not already base64/fixed-length.
public static class JwtSigningKeyFactory
{
    public static SymmetricSecurityKey CreateKey(string configuredKey)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(configuredKey));
        return new SymmetricSecurityKey(bytes);
    }
}
