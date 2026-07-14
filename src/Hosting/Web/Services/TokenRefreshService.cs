using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;

namespace CelebrationPassports.Web.Services;

// Deliberately given its own HttpClient with no BearerTokenHandler attached — this call
// carries the refresh token, not an access token, and must not go through the handler
// that this service is itself invoked from (BearerTokenHandler), to avoid re-entrancy.
public class TokenRefreshService : ITokenRefreshService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public TokenRefreshService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<RefreshedTokens?> RefreshAsync(string refreshToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Authentication/refresh", new { refreshToken });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<RefreshResponseBody>(JsonOptions);

        if (body is null || string.IsNullOrEmpty(body.AccessToken))
        {
            return null;
        }

        return new RefreshedTokens
        {
            AccessToken = body.AccessToken,
            RefreshToken = body.RefreshToken,
            ExpiresOn = body.ExpiresOn
        };
    }

    private sealed class RefreshResponseBody
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
    }
}
