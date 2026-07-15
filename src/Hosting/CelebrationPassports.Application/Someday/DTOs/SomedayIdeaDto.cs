namespace CelebrationPassports.Application.Someday.DTOs;

public class SomedayIdeaDto
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? ConvertedToEventId { get; set; }
}
