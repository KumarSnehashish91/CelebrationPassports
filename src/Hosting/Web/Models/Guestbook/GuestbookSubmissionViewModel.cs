namespace CelebrationPassports.Web.Models.Guestbook;

public class GuestbookSubmissionViewModel
{
    public Guid Id { get; set; }

    public Guid ChapterId { get; set; }

    public string GuestName { get; set; } = string.Empty;

    public string? Message { get; set; }

    public string? PhotoUrl { get; set; }

    public DateTime CreatedAt { get; set; }
}
