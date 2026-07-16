namespace CelebrationPassports.Application.GiftStories.DTOs;

public class StoryParagraphDto
{
    public string Text { get; set; } = string.Empty;

    // "User" | "Ai"
    public string Origin { get; set; } = string.Empty;

    public Guid? SourcePhotoId { get; set; }
}
