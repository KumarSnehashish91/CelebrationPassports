using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Stories;

namespace CelebrationPassports.Web.Services;

// Turns a story's chapter list (titles, categories, dates — no per-photo captions exist
// in this app) into a short narrative essay via the local Ollama model. Same "honest
// about what's real" pattern as TripPlannerService: the model only ever sees the
// structured facts we actually have, never invented details, and the page says plainly
// that it's AI-generated.
public class StoryNarrativeService : IStoryNarrativeService
{
    private readonly IAIService _aiService;

    public StoryNarrativeService(IAIService aiService)
    {
        _aiService = aiService;
    }

    public async Task<string?> GenerateAsync(StoryDetailViewModel story)
    {
        var prompt = BuildPrompt(story);
        var raw = await _aiService.GenerateAsync(prompt);

        return string.IsNullOrWhiteSpace(raw) ? null : raw.Trim();
    }

    private static string BuildPrompt(StoryDetailViewModel story)
    {
        var placeClause = string.IsNullOrWhiteSpace(story.PlaceName) ? "" : $" in {story.PlaceName}";

        var dateClause = story.StartDate.HasValue
            ? $" It took place around {story.StartDate.Value:MMMM yyyy}."
            : "";

        var chapterLines = story.Chapters.Count == 0
            ? "No chapters have been added yet."
            : string.Join("\n", story.Chapters
                .OrderBy(c => c.EventDate)
                .Select(c => $"- {c.Title} ({c.CategoryName}, {c.EventDate:MMM dd, yyyy})"));

        return
            $"You are a warm, reflective memoir writer. Write a short narrative essay (3-4 short paragraphs, " +
            $"no headers, no markdown, no bullet points) about a personal story called \"{story.Title}\"{placeClause}.{dateClause}\n\n" +
            "The story is made up of these moments, in order:\n" +
            chapterLines +
            "\n\nWrite as if reflecting back on these moments together, weaving them into a single flowing " +
            "narrative. Use only the moments listed above — do not invent people, places, or events not " +
            "implied by them. Keep the tone warm and personal, not generic. Separate paragraphs with a blank line.";
    }
}
