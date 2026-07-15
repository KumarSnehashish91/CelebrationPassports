using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.TripPlanner;

namespace CelebrationPassports.Web.Services;

public class TripItineraryService : ITripItineraryService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public TripItineraryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TripPlanDayViewModel>> GetByEventAsync(Guid eventId)
    {
        var response = await _httpClient.GetAsync($"api/events/{eventId}/itinerary");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<DayBody>>(JsonOptions);

        return body?.Select(d => new TripPlanDayViewModel
        {
            DayNumber = d.DayNumber,
            Title = d.Title,
            Description = d.Description
        }).OrderBy(d => d.DayNumber).ToList() ?? [];
    }

    public async Task<bool> SaveItineraryAsync(Guid eventId, List<TripPlanDayViewModel> days)
    {
        var request = new
        {
            days = days.Select(d => new { dayNumber = d.DayNumber, title = d.Title, description = d.Description })
        };

        var response = await _httpClient.PutAsJsonAsync($"api/events/{eventId}/itinerary", request);

        return response.IsSuccessStatusCode;
    }

    private sealed class DayBody
    {
        public int DayNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
