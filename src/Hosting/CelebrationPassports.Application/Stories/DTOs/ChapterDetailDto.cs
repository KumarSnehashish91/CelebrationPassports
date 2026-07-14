using CelebrationPassports.Application.Media.DTOs;
using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Stories.DTOs;

public class ChapterDetailDto
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public Guid? StoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public Guid? PlaceId { get; set; }

    public Guid? CoverMediaId { get; set; }

    public DateOnly EventDate { get; set; }

    public int DisplayOrder { get; set; }

    public ChapterStatus Status { get; set; }

    public ChapterSource Source { get; set; }

    public List<MediaDto> Media { get; set; } = new();
}
