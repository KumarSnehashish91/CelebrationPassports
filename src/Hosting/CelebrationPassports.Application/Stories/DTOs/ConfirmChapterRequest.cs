namespace CelebrationPassports.Application.Stories.DTOs;

public class ConfirmChapterRequest
{
    public string Title { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public Guid? PlaceId { get; set; }

    public DateOnly EventDate { get; set; }

    // Pick an existing Story to roll this chapter into. Leave null to create a new one
    // (using NewStoryTitle if given, otherwise an AI-suggested title based on place/date).
    public Guid? ExistingStoryId { get; set; }

    public string? NewStoryTitle { get; set; }
}
