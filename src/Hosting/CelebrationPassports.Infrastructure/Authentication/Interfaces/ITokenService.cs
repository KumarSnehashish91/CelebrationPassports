namespace CelebrationPassports.Infrastructure.Authentication.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, Guid sessionId);

    string GenerateRefreshToken();

    DateTime GetAccessTokenExpiry();

    DateTime GetRefreshTokenExpiry();
}
