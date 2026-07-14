namespace CelebrationPassports.Application.Stories.DTOs;

public class CreateStoryRequest
{
    public string Title { get; set; } = string.Empty;

    public Guid? PlaceId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}
