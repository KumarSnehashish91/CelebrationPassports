using CelebrationPassports.Application.Notifications.DTOs;
using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Notifications.Interfaces;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> ListMineAsync(Guid userId, int take = 20);

    Task<int> CountUnreadAsync(Guid userId);

    Task MarkAsReadAsync(Guid userId, Guid notificationId);

    // Used internally by other services (e.g. trip detection) — not exposed as its own
    // API endpoint, notifications are always a side effect of something else happening.
    Task CreateAsync(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        ReferenceType? referenceType = null,
        Guid? referenceId = null,
        string? actionUrl = null);
}
