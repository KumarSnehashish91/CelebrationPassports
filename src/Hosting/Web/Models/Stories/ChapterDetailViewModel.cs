namespace CelebrationPassports.Web.Models.Stories;

public class ChapterDetailViewModel
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    // Null while still a Draft (auto-detected, unconfirmed).
    public Guid? StoryId { get; set; }

    public string StoryTitle { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public Guid? PlaceId { get; set; }

    public DateOnly EventDate { get; set; }

    // Mirrors CelebrationPassports.Persistence.Enums.ChapterStatus (Draft=1, Confirmed=2).
    public int Status { get; set; }

    // Mirrors CelebrationPassports.Persistence.Enums.ChapterSource (Manual=1, AiDetected=2).
    public int Source { get; set; }

    public List<MediaItemViewModel> Media { get; set; } = [];
}
