namespace CelebrationPassports.Web.Models.Dashboard;

public class UpcomingCelebration
{
    public string Title { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
}