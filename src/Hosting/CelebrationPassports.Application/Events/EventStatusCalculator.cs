using System.Text.RegularExpressions;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Events;

// Upcoming/Ongoing/Completed are derived from the event's own date/time on every read
// rather than stored — the DB row keeps whatever status Finish set (Upcoming) forever,
// and this recomputes what that means "right now". Draft is the one status a user sets
// explicitly (via the wizard) and is never overridden here. Events with no explicit end
// are treated as lasting until the end of their (end date or start date) day, so a
// same-day event without an end time still reads as Ongoing all day rather than for a
// single instant.
public static class EventStatusCalculator
{
    // TimeZoneId is stored as the wizard's dropdown display text, e.g.
    // "(GMT+05:30) India Standard Time (IST)" — StartDate/StartTime are entered as that
    // zone's local wall-clock time, not UTC. Comparing them straight against UTC-now
    // (as this used to) makes every event in a positive-offset zone look like it starts
    // hours later than it really does — e.g. a 3 PM IST event reads as "upcoming" against
    // 9:30 AM UTC. Defaults to IST since that's the wizard's own default and this app's
    // primary audience.
    private static readonly Regex OffsetPattern = new(@"GMT([+-])(\d{2}):(\d{2})", RegexOptions.Compiled);
    private static readonly TimeSpan DefaultOffset = new(5, 30, 0);

    public static EventStatus GetEffectiveStatus(Event @event, DateTime nowUtc)
    {
        return GetEffectiveStatus(
            @event.Status,
            @event.StartDate,
            @event.StartTime,
            @event.EndDate,
            @event.EndTime,
            @event.TimeZoneId,
            nowUtc);
    }

    public static EventStatus GetEffectiveStatus(
        EventStatus storedStatus,
        DateOnly startDate,
        TimeOnly? startTime,
        DateOnly? endDate,
        TimeOnly? endTime,
        string? timeZoneId,
        DateTime nowUtc)
    {
        if (storedStatus == EventStatus.Draft)
        {
            return EventStatus.Draft;
        }

        var offset = ParseUtcOffset(timeZoneId);

        var start = new DateTimeOffset(startDate.ToDateTime(startTime ?? TimeOnly.MinValue), offset);
        var end = new DateTimeOffset((endDate ?? startDate).ToDateTime(endTime ?? TimeOnly.MaxValue), offset);
        var now = new DateTimeOffset(DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc));

        if (now < start)
        {
            return EventStatus.Upcoming;
        }

        if (now > end)
        {
            return EventStatus.Completed;
        }

        return EventStatus.Ongoing;
    }

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
