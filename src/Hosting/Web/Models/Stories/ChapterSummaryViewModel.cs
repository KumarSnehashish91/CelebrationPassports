namespace CelebrationPassports.Web.Models.Stories;

public class ChapterSummaryViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public DateOnly EventDate { get; set; }

    public string? CoverImageUrl { get; set; }

    public int MediaCount { get; set; }
}
