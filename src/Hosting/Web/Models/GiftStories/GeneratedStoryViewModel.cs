namespace CelebrationPassports.Web.Models.GiftStories;

public class StoryParagraphViewModel
{
    public string Text { get; set; } = string.Empty;

    public string Origin { get; set; } = string.Empty;

    public Guid? SourcePhotoId { get; set; }

    public bool IsAiWritten => Origin == "Ai";
}

public class GeneratedStoryViewModel
{
    public string Title { get; set; } = string.Empty;

    public string OpeningParagraph { get; set; } = string.Empty;

    public string ClosingParagraph { get; set; } = string.Empty;

    public string? PullQuoteText { get; set; }

    public string? PullQuoteOrigin { get; set; }

    public List<StoryParagraphViewModel> BodyParagraphs { get; set; } = [];

    public int RegenerationCount { get; set; }

    public int UserSourcedCount => BodyParagraphs.Count(p => p.Origin == "User") + (PullQuoteOrigin == "User" ? 1 : 0);

    public int AiSourcedCount => BodyParagraphs.Count(p => p.Origin == "Ai") + (PullQuoteOrigin == "Ai" ? 1 : 0) + 2;
}
