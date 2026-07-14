namespace CelebrationPassports.Application.Stories.DTOs;

public class UpdateChapterRequest
{
    public string Title { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public Guid? PlaceId { get; set; }

    public DateOnly EventDate { get; set; }

    public int DisplayOrder { get; set; }
}
