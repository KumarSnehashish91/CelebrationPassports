using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Recap;

namespace CelebrationPassports.Web.Services;

// Feature: Private Annual Recap (feature-backlog-v1.1.md, RELIVE #7). Same honest
// pattern as StoryNarrativeService/TripPlannerService — the model only sees the real
// moments (title, category, date) that happened in the requested year, generated
// on-demand rather than cached, per the backlog's own scoping note.
public class RecapService : IRecapService
{
    private readonly IAIService _aiService;

    public RecapService(IAIService aiService)
    {
        _aiService = aiService;
    }

    public async Task<string?> GenerateAsync(int year, List<RecapMomentViewModel> moments)
    {
        var prompt = BuildPrompt(year, moments);
        var raw = await _aiService.GenerateAsync(prompt);

        return string.IsNullOrWhiteSpace(raw) ? null : raw.Trim();
    }

    private static string BuildPrompt(int year, List<RecapMomentViewModel> moments)
    {
        var momentLines = string.Join("\n", moments
            .OrderBy(m => m.EventDate)
            .Select(m => $"- {m.EventDate:MMM dd}: {m.Title} ({m.CategoryName}, part of \"{m.StoryTitle}\")"));

        return
            $"You are a warm, reflective memoir writer. Write a short year-in-review essay (4-5 short " +
            $"paragraphs, no headers, no markdown, no bullet points) reflecting back on {year}, based only " +
            "on the real moments listed below, in chronological order:\n\n" +
            momentLines +
            "\n\nWeave these into a single flowing narrative of the year, as if reflecting back on it as a " +
            "whole. Use only the moments listed above — do not invent people, places, or events not implied " +
            "by them, and do not invent a theme or arc that isn't supported by them. Keep the tone warm and " +
            "personal, not generic. Separate paragraphs with a blank line.";
    }
}
