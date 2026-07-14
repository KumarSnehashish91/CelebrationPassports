namespace CelebrationPassports.Web.Models.Stories;

public class ChapterDetailViewModel
{
    public Guid Id { get; set; }

    public Guid StoryId { get; set; }

    public string StoryTitle { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public DateOnly EventDate { get; set; }

    public List<MediaItemViewModel> Media { get; set; } = [];
}
