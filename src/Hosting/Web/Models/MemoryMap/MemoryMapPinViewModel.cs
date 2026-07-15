namespace CelebrationPassports.Web.Models.MemoryMap;

public class MemoryMapPinViewModel
{
    public Guid ChapterId { get; set; }

    public Guid? StoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateOnly EventDate { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public string? PlaceName { get; set; }

    public int PhotoCount { get; set; }
}
