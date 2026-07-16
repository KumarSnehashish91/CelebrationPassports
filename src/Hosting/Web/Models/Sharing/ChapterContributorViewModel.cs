namespace CelebrationPassports.Web.Models.Sharing;

public class ChapterContributorViewModel
{
    public Guid Id { get; set; }

    public Guid ChapterId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
