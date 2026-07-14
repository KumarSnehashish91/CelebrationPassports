using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Account;

namespace CelebrationPassports.Web.Services;

public class AuthenticationService : IAuthenticationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResult> RegisterAsync(RegisterViewModel model)
    {
        var request = new
        {
            firstName = model.FirstName,
            lastName = model.LastName,
            email = model.Email,
            password = model.Password,
            confirmPassword = model.ConfirmPassword
        };

        var response = await _httpClient.PostAsJsonAsync("api/Authentication/register", request);

        if (!response.IsSuccessStatusCode)
        {
            return await BuildFailureResultAsync(response);
        }

        var body = await response.Content.ReadFromJsonAsync<RegisterResponseBody>(JsonOptions);

        if (body is null)
        {
            return new AuthResult { Success = false, ErrorMessage = "Unexpected response from the server." };
        }

        return new AuthResult
        {
            Success = true,
            UserId = body.Id,
            Email = body.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            AccessToken = body.AccessToken,
            RefreshToken = body.RefreshToken,
            ExpiresOn = body.ExpiresOn,
            SessionId = body.SessionId
        };
    }

    public async Task<AuthResult> LoginAsync(LoginViewModel model)
    {
        var request = new
        {
            emailAddress = model.Email,
            password = model.Password
        };

        var response = await _httpClient.PostAsJsonAsync("api/Authentication/login", request);

        if (!response.IsSuccessStatusCode)
        {
            return await BuildFailureResultAsync(response);
        }

        var body = await response.Content.ReadFromJsonAsync<LoginResponseBody>(JsonOptions);

        if (body is null)
        {
            return new AuthResult { Success = false, ErrorMessage = "Unexpected response from the server." };
        }

        return new AuthResult
        {
            Success = true,
            UserId = body.UserId,
            Email = body.EmailAddress,
            FirstName = body.FirstName,
            LastName = body.LastName,
            AccessToken = body.AccessToken,
            RefreshToken = body.RefreshToken,
            ExpiresOn = body.ExpiresOn,
            SessionId = body.SessionId
        };
    }

    public async Task LogoutAsync(Guid sessionId)
    {
        // Best-effort — the caller clears the local cookie regardless of whether this succeeds.
        try
        {
            await _httpClient.PostAsJsonAsync("api/Authentication/logout", new { sessionId });
        }
        catch (HttpRequestException)
        {
        }
    }

    private static async Task<AuthResult> BuildFailureResultAsync(HttpResponseMessage response)
    {
        var errorMessage = "Something went wrong. Please try again.";

        try
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponseBody>(JsonOptions);

            if (!string.IsNullOrWhiteSpace(error?.Message))
            {
                errorMessage = error.Message;
            }
        }
        catch (JsonException)
        {
        }

        return new AuthResult { Success = false, ErrorMessage = errorMessage };
    }

    private sealed class RegisterResponseBody
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
        public Guid SessionId { get; set; }
    }

    private sealed class LoginResponseBody
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
        public Guid SessionId { get; set; }
    }

    private sealed class ErrorResponseBody
    {
        public string? Message { get; set; }
    }
}
