namespace CelebrationPassports.Application.TimeCapsule.DTOs;

public class TimeCapsuleMessageDto
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public Guid AuthorUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    // Null while locked for the requesting user — withheld server-side, not just
    // hidden by the UI. Always populated for the author, and for anyone once unlocked.
    public string? Content { get; set; }

    public DateTime UnlockDate { get; set; }

    public bool IsUnlocked { get; set; }

    public DateTime CreatedAt { get; set; }
}
