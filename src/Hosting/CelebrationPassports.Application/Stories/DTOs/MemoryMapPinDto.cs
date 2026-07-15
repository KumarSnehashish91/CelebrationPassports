namespace CelebrationPassports.Application.Stories.DTOs;

// Feature: Memory Map (feature-backlog-v1.1.md, PRESERVE #4). One pin per Chapter
// ("PassportMoment" in the spec) that has a resolved, geo-tagged Place — reuses the
// same Place-resolution already populated by auto-chapter clustering and trip
// detection, rather than pinning every individual geo-tagged photo (which would be far
// too dense to be useful on a map).
public class MemoryMapPinDto
{
    public Guid ChapterId { get; set; }

    public Guid? StoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateOnly EventDate { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public string? PlaceName { get; set; }

    public int PhotoCount { get; set; }

    public Guid? CoverMediaId { get; set; }
}
