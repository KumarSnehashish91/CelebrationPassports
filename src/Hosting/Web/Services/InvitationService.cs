using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Invitations;

namespace CelebrationPassports.Web.Services;

public class InvitationService : IInvitationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public InvitationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<InvitationViewModel>> GetPendingAsync()
    {
        var response = await _httpClient.GetAsync("api/invitations/mine");
        return await ReadInvitationsAsync(response);
    }

    public async Task<List<InvitationViewModel>> GetByPassportAsync(Guid passportId)
    {
        var response = await _httpClient.GetAsync($"api/passports/{passportId}/invitations");
        return await ReadInvitationsAsync(response);
    }

    public async Task<bool> InviteAsync(Guid passportId, string email)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/passports/{passportId}/invitations", new { email });

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AcceptAsync(Guid invitationId)
    {
        var response = await _httpClient.PostAsync($"api/invitations/{invitationId}/accept", null);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeclineAsync(Guid invitationId)
    {
        var response = await _httpClient.PostAsync($"api/invitations/{invitationId}/decline", null);

        return response.IsSuccessStatusCode;
    }

    private static async Task<List<InvitationViewModel>> ReadInvitationsAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<InvitationBody>>(JsonOptions);

        return body?.Select(i => new InvitationViewModel
        {
            Id = i.Id,
            PassportTitle = i.PassportTitle,
            Email = i.Email,
            Status = MapStatus(i.Status)
        }).ToList() ?? [];
    }

    // Mirrors CelebrationPassports.Persistence.Enums.PassportInvitationStatus
    // (Pending=1, Accepted=2, Declined=3, Expired=4).
    private static string MapStatus(int status) => status switch
    {
        1 => "Pending",
        2 => "Accepted",
        3 => "Declined",
        4 => "Expired",
        _ => "Unknown"
    };

    private sealed class InvitationBody
    {
        public Guid Id { get; set; }
        public string PassportTitle { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}
