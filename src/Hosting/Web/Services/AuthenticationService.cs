using System.Text;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Account;

namespace CelebrationPassports.Web.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> RegisterAsync(RegisterViewModel model)
    {
        var request = new
        {
            email = model.Email,
            password = model.Password,
            confirmPassword = model.ConfirmPassword
        };

        var json = JsonSerializer.Serialize(request);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(
            "api/Authentication/register",
            content);

        return response.IsSuccessStatusCode;
    }
}