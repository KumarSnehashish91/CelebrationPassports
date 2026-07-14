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
}
