using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Sharing.DTOs;

public class ChapterInvitationDto
{
    public Guid Id { get; set; }

    public Guid ChapterId { get; set; }

    public string ChapterTitle { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public ChapterInvitationStatus Status { get; set; }
}
