using System.Text.RegularExpressions;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.TripPlanner;

namespace CelebrationPassports.Web.Services;

// Deliberately simple/heuristic parsing of the AI's plain-text response — the local
// Ollama model behind IAIService has no structured-output mode here, so this asks for a
// specific line-based format and falls back to showing the raw text if the model doesn't
// follow it, rather than crashing or showing a blank page. Photos are generic
// placeholder imagery (picsum.photos, seeded by destination so the same place looks the
// same across requests) — there's no real photo-search API in this project, same
// "honest about what's real" pattern as TripDetectionService.
public class TripPlannerService : ITripPlannerService
{
    private static readonly Regex DayPattern = new(
        @"^DAY\s*(\d+)\s*:\s*(.+?)\s*\|\s*(.+)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly IAIService _aiService;

    public TripPlannerService(IAIService aiService)
    {
        _aiService = aiService;
    }

    public async Task<TripPlanViewModel?> GenerateAsync(string destination, int days, string? notes)
    {
        var prompt = BuildPrompt(destination, days, notes);
        var raw = await _aiService.GenerateAsync(prompt);

        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var lines = raw.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var overviewLine = lines.FirstOrDefault(l => l.StartsWith("OVERVIEW:", StringComparison.OrdinalIgnoreCase));
        var overview = overviewLine is null ? null : overviewLine["OVERVIEW:".Length..].Trim();

        var itinerary = lines
            .Select(l => DayPattern.Match(l))
            .Where(m => m.Success)
            .Select(m => new TripPlanDayViewModel
            {
                DayNumber = int.Parse(m.Groups[1].Value),
                Title = m.Groups[2].Value.Trim(),
                Description = m.Groups[3].Value.Trim()
            })
            .OrderBy(d => d.DayNumber)
            .ToList();

        // The model didn't follow the requested format — fall back to showing whatever
        // it actually said rather than an empty result.
        if (string.IsNullOrWhiteSpace(overview) && itinerary.Count == 0)
        {
            overview = raw.Trim();
        }

        return new TripPlanViewModel
        {
            Destination = destination,
            Days = days,
            Notes = notes,
            Overview = overview,
            Itinerary = itinerary,
            PhotoUrls = BuildPhotoUrls(destination)
        };
    }

    private static string BuildPrompt(string destination, int days, string? notes)
    {
        var notesClause = string.IsNullOrWhiteSpace(notes) ? "" : $" Traveler preferences: {notes.Trim()}.";

        var dayLines = string.Join("\n", Enumerable.Range(1, days)
            .Select(d => $"DAY {d}: <short title> | <2-3 sentence description of activities>"));

        return
            $"You are a trip planning assistant. Create a {days}-day travel itinerary for a trip to {destination}.{notesClause}\n\n" +
            "Respond in EXACTLY this format, with no other text, headers, or markdown:\n" +
            "OVERVIEW: <2-3 sentence overview of the trip>\n" +
            dayLines +
            "\n\nReplace each <...> with real content specific to the destination. Keep each day on one line.";
    }

    private static List<string> BuildPhotoUrls(string destination)
    {
        var seed = Uri.EscapeDataString(destination.Trim().ToLowerInvariant());

        return Enumerable.Range(1, 5)
            .Select(i => $"https://picsum.photos/seed/{seed}-{i}/400/300")
            .ToList();
    }
}
