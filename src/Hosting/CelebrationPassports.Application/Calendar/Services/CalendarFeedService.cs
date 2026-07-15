using System.Text;
using System.Text.RegularExpressions;
using CelebrationPassports.Application.Calendar.Interfaces;
using CelebrationPassports.Application.Events.DTOs;
using CelebrationPassports.Application.Events.Interfaces;
using CelebrationPassports.Application.Places.Interfaces;
using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Calendar.Services;

public class CalendarFeedService : ICalendarFeedService
{
    // Same "(GMT+05:30) India Standard Time (IST)" wall-clock-offset parsing as
    // EventStatusCalculator — duplicated rather than shared, same small-utility
    // duplication convention used elsewhere in this codebase (e.g. the haversine
    // distance calc in TripDetectionService / AutoChapterClusteringService).
    private static readonly Regex OffsetPattern = new(@"GMT([+-])(\d{2}):(\d{2})", RegexOptions.Compiled);
    private static readonly TimeSpan DefaultOffset = new(5, 30, 0);

    private readonly IEventService _eventService;
    private readonly IPlaceService _placeService;

    public CalendarFeedService(IEventService eventService, IPlaceService placeService)
    {
        _eventService = eventService;
        _placeService = placeService;
    }

    public async Task<string> GenerateIcsAsync(Guid userId)
    {
        var summaries = await _eventService.GetAllForUserAsync(userId, null);

        var relevantIds = summaries
            .Where(e => e.Status is EventStatus.Upcoming or EventStatus.Ongoing)
            .OrderBy(e => e.StartDate)
            .Select(e => e.Id)
            .ToList();

        var sb = new StringBuilder();
        sb.Append("BEGIN:VCALENDAR\r\n");
        sb.Append("VERSION:2.0\r\n");
        sb.Append("PRODID:-//CelebrationPassports//Calendar Feed//EN\r\n");
        sb.Append("CALSCALE:GREGORIAN\r\n");
        sb.Append("METHOD:PUBLISH\r\n");
        sb.Append("X-WR-CALNAME:Celebration Passports\r\n");

        var placeNameCache = new Dictionary<Guid, string?>();

        foreach (var id in relevantIds)
        {
            EventDetailDto detail;

            try
            {
                detail = await _eventService.GetByIdAsync(userId, id);
            }
            catch
            {
                // Best-effort feed — skip anything that fails to load rather than
                // break the whole subscription for one bad event.
                continue;
            }

            string? placeName = null;

            if (detail.PlaceId.HasValue)
            {
                if (!placeNameCache.TryGetValue(detail.PlaceId.Value, out placeName))
                {
                    placeName = await ResolvePlaceNameAsync(detail.PlaceId.Value);
                    placeNameCache[detail.PlaceId.Value] = placeName;
                }
            }

            AppendEvent(sb, detail, placeName);
        }

        sb.Append("END:VCALENDAR\r\n");

        return sb.ToString();
    }

    private async Task<string?> ResolvePlaceNameAsync(Guid placeId)
    {
        try
        {
            var place = await _placeService.GetByIdAsync(placeId);
            return place is null
                ? null
                : place.Name + (!string.IsNullOrWhiteSpace(place.City) ? $", {place.City}" : "");
        }
        catch
        {
            return null;
        }
    }

    private static void AppendEvent(StringBuilder sb, EventDetailDto detail, string? placeName)
    {
        sb.Append("BEGIN:VEVENT\r\n");
        sb.Append($"UID:{detail.Id}@celebrationpassports\r\n");
        sb.Append($"DTSTAMP:{DateTime.UtcNow:yyyyMMdd'T'HHmmss'Z'}\r\n");

        if (detail.IsAllDay || detail.StartTime is null)
        {
            var start = detail.StartDate;
            // DTEND is exclusive for all-day VEVENTs, so the last day needs +1.
            var end = (detail.EndDate ?? detail.StartDate).AddDays(1);

            sb.Append($"DTSTART;VALUE=DATE:{start:yyyyMMdd}\r\n");
            sb.Append($"DTEND;VALUE=DATE:{end:yyyyMMdd}\r\n");
        }
        else
        {
            var offset = ParseUtcOffset(detail.TimeZoneId);

            var startUtc = new DateTimeOffset(detail.StartDate.ToDateTime(detail.StartTime.Value), offset).UtcDateTime;

            var endDateOnly = detail.EndDate ?? detail.StartDate;
            var endTime = detail.EndTime ?? detail.StartTime.Value.Add(TimeSpan.FromHours(1));
            var endUtc = new DateTimeOffset(endDateOnly.ToDateTime(endTime), offset).UtcDateTime;

            if (endUtc <= startUtc)
            {
                endUtc = startUtc.AddHours(1);
            }

            sb.Append($"DTSTART:{startUtc:yyyyMMdd'T'HHmmss'Z'}\r\n");
            sb.Append($"DTEND:{endUtc:yyyyMMdd'T'HHmmss'Z'}\r\n");
        }

        sb.Append($"SUMMARY:{Escape(detail.Title)}\r\n");

        if (!string.IsNullOrWhiteSpace(placeName))
        {
            sb.Append($"LOCATION:{Escape(placeName)}\r\n");
        }

        if (!string.IsNullOrWhiteSpace(detail.Notes))
        {
            sb.Append($"DESCRIPTION:{Escape(detail.Notes)}\r\n");
        }

        sb.Append("END:VEVENT\r\n");
    }

    // RFC 5545 §3.3.11 text escaping.
    private static string Escape(string text) => text
        .Replace("\\", "\\\\")
        .Replace(";", "\\;")
        .Replace(",", "\\,")
        .Replace("\r\n", "\\n")
        .Replace("\n", "\\n");

    private static TimeSpan ParseUtcOffset(string? timeZoneId)
    {
        if (timeZoneId is not null)
        {
            var match = OffsetPattern.Match(timeZoneId);

            if (match.Success)
            {
                var hours = int.Parse(match.Groups[2].Value);
                var minutes = int.Parse(match.Groups[3].Value);
                var magnitude = new TimeSpan(hours, minutes, 0);

                return match.Groups[1].Value == "-" ? -magnitude : magnitude;
            }
        }

        return DefaultOffset;
    }
}
