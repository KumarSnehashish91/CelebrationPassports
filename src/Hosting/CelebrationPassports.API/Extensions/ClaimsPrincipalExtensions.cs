using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CelebrationPassports.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (value is null || !Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException("The current user could not be identified.");
        }

        return userId;
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        // JwtTokenService issues this as the short "email" claim (JwtRegisteredClaimNames.Email).
        // Whether that arrives as-is or gets remapped to the long ClaimTypes.Email URI depends on
        // the JWT bearer handler's inbound claim-mapping behavior, so check both.
        var value = user.FindFirstValue(JwtRegisteredClaimNames.Email)
            ?? user.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(value))
        {
            throw new UnauthorizedAccessException("The current user's email could not be identified.");
        }

        return value;
    }
}
