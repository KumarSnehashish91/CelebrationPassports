using CelebrationPassports.Web.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;

namespace CelebrationPassports.Web.Handlers;

// Attaches the JWT access token stored in the auth cookie (via SignInAsync's
// AuthenticationProperties.StoreTokens) to outgoing calls to the API. Access tokens are
// short-lived (60 min, see API appsettings Jwt:AccessTokenMinutes) and were previously
// never refreshed, so any session older than an hour started silently 401ing on every
// API call — every Web service here swallows non-success responses into null/false,
// which surfaced as confusing, unrelated-looking failures ("can't create/edit events",
// "passport could not be generated") rather than a clear "please log in again". This
// handler now proactively refreshes the token before it expires, keeping long sessions
// working transparently, and only signs the user out if the refresh token itself is
// no longer valid.
public class BearerTokenHandler : DelegatingHandler
{
    // Serializes concurrent refresh attempts (e.g. Dashboard fires several parallel API
    // calls) so they don't race to rotate the same refresh token against each other.
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenRefreshService _tokenRefreshService;

    public BearerTokenHandler(IHttpContextAccessor httpContextAccessor, ITokenRefreshService tokenRefreshService)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenRefreshService = tokenRefreshService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext != null)
        {
            var accessToken = await GetValidAccessTokenAsync(httpContext);

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string?> GetValidAccessTokenAsync(HttpContext httpContext)
    {
        var accessToken = await httpContext.GetTokenAsync("access_token");

        if (string.IsNullOrEmpty(accessToken) || !IsExpiringSoon(await httpContext.GetTokenAsync("expires_on")))
        {
            return accessToken;
        }

        await RefreshLock.WaitAsync();

        try
        {
            // Re-read after acquiring the lock — another request may have already
            // refreshed the token while this one was waiting.
            accessToken = await httpContext.GetTokenAsync("access_token");

            if (!IsExpiringSoon(await httpContext.GetTokenAsync("expires_on")))
            {
                return accessToken;
            }

            var refreshToken = await httpContext.GetTokenAsync("refresh_token");

            if (string.IsNullOrEmpty(refreshToken))
            {
                return accessToken;
            }

            var refreshed = await _tokenRefreshService.RefreshAsync(refreshToken);

            if (refreshed is null)
            {
                // The refresh token is invalid or expired too — nothing left to do but
                // force a clean re-login instead of letting every call keep 401ing.
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return null;
            }

            await ReSignInWithRefreshedTokensAsync(httpContext, refreshed);
            return refreshed.AccessToken;
        }
        finally
        {
            RefreshLock.Release();
        }
    }

    private static bool IsExpiringSoon(string? expiresOnRaw)
    {
        return !DateTimeOffset.TryParse(expiresOnRaw, out var expiresOn)
            || expiresOn <= DateTimeOffset.UtcNow.AddSeconds(30);
    }

    private static async Task ReSignInWithRefreshedTokensAsync(HttpContext httpContext, RefreshedTokens refreshed)
    {
        var authenticateResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (authenticateResult?.Succeeded != true || authenticateResult.Principal is null)
        {
            return;
        }

        var properties = authenticateResult.Properties ?? new AuthenticationProperties();
        properties.StoreTokens(new[]
        {
            new AuthenticationToken { Name = "access_token", Value = refreshed.AccessToken },
            new AuthenticationToken { Name = "refresh_token", Value = refreshed.RefreshToken },
            new AuthenticationToken { Name = "expires_on", Value = refreshed.ExpiresOn.ToString("o") }
        });

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticateResult.Principal, properties);
    }
}
