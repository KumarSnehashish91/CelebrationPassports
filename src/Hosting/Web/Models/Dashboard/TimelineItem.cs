namespace CelebrationPassports.Web.Models.Dashboard;

public class TimelineItem
{
    public int Year { get; set; }

    public string Month { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string StampColor { get; set; } = "#D4AF37";

    public Guid StoryId { get; set; }
}