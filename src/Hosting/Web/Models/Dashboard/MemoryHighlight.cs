namespace CelebrationPassports.Web.Models.Dashboard;

public class MemoryHighlight
{
    public string Title { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public int Photos { get; set; }

    public int Videos { get; set; }

    public int People { get; set; }

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
}