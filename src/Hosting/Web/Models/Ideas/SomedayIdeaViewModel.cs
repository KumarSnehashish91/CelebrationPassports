namespace CelebrationPassports.Web.Models.Ideas;

public class SomedayIdeaViewModel
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? ConvertedToEventId { get; set; }
}
