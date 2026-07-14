using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Passports;

namespace CelebrationPassports.Web.Services;

public class PassportService : IPassportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public PassportService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PassportListItemViewModel>> GetMineAsync()
    {
        var response = await _httpClient.GetAsync("api/Passports");

        if (!response.IsSuccessStatusCode)
        {
            return new List<PassportListItemViewModel>();
        }

        var body = await response.Content.ReadFromJsonAsync<List<PassportSummaryBody>>(JsonOptions);

        return body?.Select(p => new PassportListItemViewModel
        {
            Id = p.Id,
            Title = p.Title,
            Status = MapStatus(p.Status),
            PeopleCount = p.PeopleCount,
            IsOwner = p.IsOwner
        }).ToList() ?? new List<PassportListItemViewModel>();
    }

    public async Task<bool> CreateAsync(CreatePassportViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Passports", new { title = model.Title });

        return response.IsSuccessStatusCode;
    }

    public async Task<int> GetStampCountAsync()
    {
        var response = await _httpClient.GetAsync("api/passport-stamps/mine/count");

        if (!response.IsSuccessStatusCode)
        {
            return 0;
        }

        var body = await response.Content.ReadFromJsonAsync<StampCountBody>(JsonOptions);

        return body?.Count ?? 0;
    }

    private sealed class StampCountBody
    {
        public int Count { get; set; }
    }

    // Mirrors CelebrationPassports.Persistence.Enums.PassportStatus (Draft=1, Active=2,
    // Archived=3) — duplicated as a plain mapping since Web no longer references
    // Persistence directly.
    private static string MapStatus(int status) => status switch
    {
        1 => "Draft",
        2 => "Active",
        3 => "Archived",
        _ => "Unknown"
    };

    private sealed class PassportSummaryBody
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public int PeopleCount { get; set; }
        public bool IsOwner { get; set; }
    }
}
