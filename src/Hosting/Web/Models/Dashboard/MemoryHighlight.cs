namespace CelebrationPassports.Web.Models.Dashboard;

public class MemoryHighlight
{
    public string Title { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public int Photos { get; set; }

    public int Videos { get; set; }

    public int Chapters { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public Guid? StoryId { get; set; }
}