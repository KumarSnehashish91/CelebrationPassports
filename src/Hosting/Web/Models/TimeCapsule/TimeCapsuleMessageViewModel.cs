namespace CelebrationPassports.Web.Models.TimeCapsule;

public class TimeCapsuleMessageViewModel
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public Guid AuthorUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    public DateTime UnlockDate { get; set; }

    public bool IsUnlocked { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsMine { get; set; }
}
