namespace CelebrationPassports.Web.Models.Recap;

public class RecapMomentViewModel
{
    public string Title { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public DateOnly EventDate { get; set; }

    public string StoryTitle { get; set; } = string.Empty;
}
