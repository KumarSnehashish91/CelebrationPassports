using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Notifications.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }

    public NotificationType NotificationType { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public ReferenceType? ReferenceType { get; set; }

    public Guid? ReferenceId { get; set; }

    public bool IsRead { get; set; }

    public string? ActionUrl { get; set; }

    public DateTime CreatedOn { get; set; }
}
