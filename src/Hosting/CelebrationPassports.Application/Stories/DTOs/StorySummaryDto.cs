namespace CelebrationPassports.Application.Stories.DTOs;

public class StorySummaryDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public Guid? PlaceId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int DisplayOrder { get; set; }

    public int ChapterCount { get; set; }
}
