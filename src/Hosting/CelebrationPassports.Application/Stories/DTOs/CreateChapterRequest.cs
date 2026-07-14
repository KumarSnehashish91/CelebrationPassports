namespace CelebrationPassports.Application.Stories.DTOs;

public class CreateChapterRequest
{
    public string Title { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public Guid? PlaceId { get; set; }

    public DateOnly EventDate { get; set; }
}
