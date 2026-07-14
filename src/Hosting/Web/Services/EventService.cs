using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Dashboard;
using CelebrationPassports.Web.Models.Events;
using CelebrationPassports.Web.Models.Places;

namespace CelebrationPassports.Web.Services;

public class EventService : IEventService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly IPlaceService _placeService;
    private readonly IMediaService _mediaService;

    public EventService(HttpClient httpClient, IPlaceService placeService, IMediaService mediaService)
    {
        _httpClient = httpClient;
        _placeService = placeService;
        _mediaService = mediaService;
    }

    public async Task<List<UpcomingCelebration>> GetUpcomingAsync(int take = 5)
    {
        var response = await _httpClient.GetAsync($"api/events/mine/upcoming?take={take}");

        if (!response.IsSuccessStatusCode)
        {
            return new List<UpcomingCelebration>();
        }

        var body = await response.Content.ReadFromJsonAsync<List<EventSummaryBody>>(JsonOptions);

        if (body is null)
        {
            return new List<UpcomingCelebration>();
        }

        var result = new List<UpcomingCelebration>();

        foreach (var e in body)
        {
            // Fall back to the placeholder only when there's genuinely no cover uploaded,
            // or resolving it fails — not as the permanent default.
            var imageUrl = e.CoverMediaId.HasValue
                ? await _mediaService.GetUrlAsync(e.CoverMediaId.Value)
                : null;

            result.Add(new UpcomingCelebration
            {
                Id = e.Id,
                Title = e.Title,
                Type = MapStatus(e.Status),
                Date = e.StartDate,
                ImageUrl = imageUrl ?? "/images/udaipur.jpg"
            });
        }

        return result;
    }

    public async Task<List<CelebrationListItemViewModel>> GetAllAsync(int? status = null)
    {
        var url = status.HasValue ? $"api/events/mine?status={status}" : "api/events/mine";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return new List<CelebrationListItemViewModel>();
        }

        var body = await response.Content.ReadFromJsonAsync<List<EventSummaryBody>>(JsonOptions);

        if (body is null)
        {
            return new List<CelebrationListItemViewModel>();
        }

        var result = new List<CelebrationListItemViewModel>();

        foreach (var e in body)
        {
            var imageUrl = e.CoverMediaId.HasValue
                ? await _mediaService.GetUrlAsync(e.CoverMediaId.Value)
                : null;

            var item = new CelebrationListItemViewModel
            {
                Id = e.Id,
                Title = e.Title,
                EventType = e.EventType,
                Status = e.Status,
                StartDate = DateOnly.FromDateTime(e.StartDate)
            };

            if (imageUrl is not null)
            {
                item.ImageUrl = imageUrl;
            }

            result.Add(item);
        }

        return result;
    }

    public async Task<EventWizardViewModel?> GetByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/events/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<EventDetailBody>(JsonOptions);

        if (body is null)
        {
            return null;
        }

        var model = new EventWizardViewModel
        {
            Id = body.Id,
            PassportId = body.PassportId,
            Status = body.Status,
            Title = body.Title,
            EventType = body.EventType,
            Description = body.Notes,
            CoverMediaId = body.CoverMediaId,
            EventDate = body.StartDate,
            StartTime = body.StartTime,
            EndTime = body.EndTime,
            IsAllDay = body.IsAllDay,
            TimeZoneId = body.TimeZoneId,
            PlaceId = body.PlaceId,
            CreatedAt = body.CreatedAt
        };

        if (body.PlaceId.HasValue)
        {
            var place = await _placeService.GetByIdAsync(body.PlaceId.Value);

            if (place is not null)
            {
                model.PlaceName = place.Name;
                model.Address = place.Address;
                model.City = place.City;
                model.PostalCode = place.PostalCode;
                model.Country = place.Country;
                model.LocationNotes = place.Notes;
            }
        }

        if (body.CoverMediaId.HasValue)
        {
            model.CoverImageUrl = await _mediaService.GetUrlAsync(body.CoverMediaId.Value);
        }

        return model;
    }

    public async Task<Guid?> SaveAsync(EventWizardViewModel model)
    {
        if (model.Id is null)
        {
            var createRequest = new
            {
                title = model.Title,
                eventType = model.EventType,
                startDate = model.EventDate ?? DateOnly.FromDateTime(DateTime.Today),
                startTime = model.StartTime,
                endTime = model.EndTime,
                isAllDay = model.IsAllDay,
                timeZoneId = model.TimeZoneId,
                placeId = model.PlaceId,
                coverMediaId = model.CoverMediaId,
                notes = model.Description
            };

            var response = await _httpClient.PostAsJsonAsync($"api/passports/{model.PassportId}/events", createRequest);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var body = await response.Content.ReadFromJsonAsync<EventDetailBody>(JsonOptions);
            return body?.Id;
        }
        else
        {
            var updateRequest = new
            {
                title = model.Title,
                eventType = model.EventType,
                status = model.Status,
                startDate = model.EventDate ?? DateOnly.FromDateTime(DateTime.Today),
                startTime = model.StartTime,
                endTime = model.EndTime,
                isAllDay = model.IsAllDay,
                timeZoneId = model.TimeZoneId,
                placeId = model.PlaceId,
                coverMediaId = model.CoverMediaId,
                notes = model.Description
            };

            var response = await _httpClient.PutAsJsonAsync($"api/events/{model.Id}", updateRequest);

            return response.IsSuccessStatusCode ? model.Id : null;
        }
    }

    public async Task<bool> FinalizeAsync(Guid id)
    {
        var model = await GetByIdAsync(id);

        if (model is null)
        {
            return false;
        }

        model.Status = 2; // EventStatus.Upcoming — actual Upcoming/Ongoing/Completed is computed server-side from the schedule.

        var result = await SaveAsync(model);
        return result is not null;
    }

    // Mirrors CelebrationPassports.Persistence.Enums.EventStatus (Draft=1, Upcoming=2,
    // Ongoing=3, Completed=4) — duplicated as a plain mapping since Web no longer
    // references Persistence directly.
    private static string MapStatus(int status) => status switch
    {
        1 => "Draft",
        2 => "Upcoming",
        3 => "Ongoing",
        4 => "Completed",
        _ => "Celebration"
    };

    private sealed class EventSummaryBody
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int EventType { get; set; }
        public int Status { get; set; }
        public DateTime StartDate { get; set; }
        public Guid? CoverMediaId { get; set; }
    }

    private sealed class EventDetailBody
    {
        public Guid Id { get; set; }
        public Guid PassportId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int EventType { get; set; }
        public int Status { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public bool IsAllDay { get; set; }
        public string? TimeZoneId { get; set; }
        public Guid? PlaceId { get; set; }
        public Guid? CoverMediaId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
