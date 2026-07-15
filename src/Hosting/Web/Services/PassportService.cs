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

    public async Task<PassportDetailViewModel?> GetByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/Passports/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<PassportDetailBody>(JsonOptions);

        if (body is null)
        {
            return null;
        }

        return new PassportDetailViewModel
        {
            Id = body.Id,
            Title = body.Title,
            Status = MapStatus(body.Status),
            OwnerUserId = body.OwnerUserId,
            CreatedOn = body.CreatedOn,
            People = body.People.Select(p => new PassportPersonViewModel
            {
                Id = p.Id,
                UserId = p.UserId,
                Name = p.Name,
                Role = MapRole(p.Role)
            }).ToList()
        };
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

    // Mirrors CelebrationPassports.Persistence.Enums.PassportPersonRole (Owner=1,
    // Editor=2, Contributor=3, Viewer=4).
    private static string MapRole(int role) => role switch
    {
        1 => "Owner",
        2 => "Editor",
        3 => "Contributor",
        4 => "Viewer",
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

    private sealed class PassportDetailBody
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public Guid OwnerUserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<PassportPersonBody> People { get; set; } = new();
    }

    private sealed class PassportPersonBody
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Role { get; set; }
    }
}
