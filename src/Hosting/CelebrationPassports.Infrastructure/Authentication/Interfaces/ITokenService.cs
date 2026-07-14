namespace CelebrationPassports.Infrastructure.Authentication.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email);

    string GenerateRefreshToken();

    DateTime GetAccessTokenExpiry();

    DateTime GetRefreshTokenExpiry();
}
