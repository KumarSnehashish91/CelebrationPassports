namespace CelebrationPassports.Web.Models.Notifications;

public class NotificationViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public string? ActionUrl { get; set; }

    public DateTime CreatedOn { get; set; }
}
