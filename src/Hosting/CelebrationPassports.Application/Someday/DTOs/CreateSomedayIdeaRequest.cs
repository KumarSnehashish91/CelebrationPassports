namespace CelebrationPassports.Application.Someday.DTOs;

public class CreateSomedayIdeaRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Notes { get; set; }
}
