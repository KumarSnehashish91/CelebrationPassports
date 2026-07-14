namespace CelebrationPassports.Web.Models.Stories;

public class StoryDetailViewModel
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? PlaceName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? CoverImageUrl { get; set; }

    public List<ChapterSummaryViewModel> Chapters { get; set; } = [];
}
