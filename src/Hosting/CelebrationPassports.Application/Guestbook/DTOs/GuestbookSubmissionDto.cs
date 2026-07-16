using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Guestbook.DTOs;

public class GuestbookSubmissionDto
{
    public Guid Id { get; set; }

    public Guid ChapterId { get; set; }

    public string GuestName { get; set; } = string.Empty;

    public string? Message { get; set; }

    public string? PhotoUrl { get; set; }

    public GuestbookSubmissionStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}
