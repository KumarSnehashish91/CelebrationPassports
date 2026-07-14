using CelebrationPassports.Application.Media.DTOs;

namespace CelebrationPassports.Application.Stories.DTOs;

public class ChapterDetailDto
{
    public Guid Id { get; set; }

    public Guid StoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public Guid? PlaceId { get; set; }

    public Guid? CoverMediaId { get; set; }

    public DateOnly EventDate { get; set; }

    public int DisplayOrder { get; set; }

    public List<MediaDto> Media { get; set; } = new();
}
