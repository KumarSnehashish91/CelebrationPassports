namespace CelebrationPassports.Web.Models.Stories;

public class StoryListItemViewModel
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? PlaceName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int ChapterCount { get; set; }

    public string? CoverImageUrl { get; set; }
}
