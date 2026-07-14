namespace CelebrationPassports.Application.Stories.DTOs;

public class StoryDetailDto
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string Title { get; set; } = string.Empty;

    public Guid? PlaceId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public Guid? CoverMediaId { get; set; }

    public int DisplayOrder { get; set; }

    public List<ChapterSummaryDto> Chapters { get; set; } = new();
}
