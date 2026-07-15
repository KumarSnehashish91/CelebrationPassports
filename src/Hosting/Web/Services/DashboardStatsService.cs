using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Dashboard;

namespace CelebrationPassports.Web.Services;

public class DashboardStatsService : IDashboardStatsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public DashboardStatsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DashboardStatsViewModel> GetSummaryAsync()
    {
        var response = await _httpClient.GetAsync("api/dashboard/stats");

        if (!response.IsSuccessStatusCode)
        {
            return new DashboardStatsViewModel();
        }

        return await response.Content.ReadFromJsonAsync<DashboardStatsViewModel>(JsonOptions)
            ?? new DashboardStatsViewModel();
    }
}
