namespace CelebrationPassports.Web.Interfaces;

public class RefreshedTokens
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ExpiresOn { get; set; }
}

public interface ITokenRefreshService
{
    Task<RefreshedTokens?> RefreshAsync(string refreshToken);
}
