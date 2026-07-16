namespace CelebrationPassports.Application.GiftStories.DTOs;

public class GeneratedStoryDto
{
    public string Title { get; set; } = string.Empty;

    public string OpeningParagraph { get; set; } = string.Empty;

    public string ClosingParagraph { get; set; } = string.Empty;

    public string? PullQuoteText { get; set; }

    // "User" | "Ai" | null
    public string? PullQuoteOrigin { get; set; }

    public List<StoryParagraphDto> BodyParagraphs { get; set; } = [];

    public int RegenerationCount { get; set; }

    public int UserSourcedCount => BodyParagraphs.Count(p => p.Origin == "User") + (PullQuoteOrigin == "User" ? 1 : 0);

    public int AiSourcedCount => BodyParagraphs.Count(p => p.Origin == "Ai") + (PullQuoteOrigin == "Ai" ? 1 : 0) + 2; // + opening + closing
}
