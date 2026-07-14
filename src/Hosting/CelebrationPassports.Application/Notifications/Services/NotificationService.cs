using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Notifications.DTOs;
using CelebrationPassports.Application.Notifications.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;

namespace CelebrationPassports.Application.Notifications.Services;

public class NotificationService : INotificationService
{
    private readonly IGenericRepository<Notification> _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IGenericRepository<Notification> notificationRepository, IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<NotificationDto>> ListMineAsync(Guid userId, int take = 20)
    {
        var notifications = await _notificationRepository.FindAsync(n => n.UserId == userId);

        return notifications
            .OrderByDescending(n => n.CreatedOn)
            .Take(take)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<int> CountUnreadAsync(Guid userId)
    {
        var notifications = await _notificationRepository.FindAsync(n => n.UserId == userId && !n.IsRead);
        return notifications.Count;
    }

    public async Task MarkAsReadAsync(Guid userId, Guid notificationId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId)
            ?? throw new NotFoundException("Notification not found.");

        if (notification.UserId != userId)
        {
            throw new ForbiddenAccessException("You don't have access to this notification.");
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadOn = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task CreateAsync(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        ReferenceType? referenceType = null,
        Guid? referenceId = null,
        string? actionUrl = null)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            NotificationType = type,
            Title = title,
            Message = message,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            ActionUrl = actionUrl,
            IsRead = false,
            CreatedOn = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();
    }

    private static NotificationDto MapToDto(Notification n) => new()
    {
        Id = n.Id,
        NotificationType = n.NotificationType,
        Title = n.Title,
        Message = n.Message,
        ReferenceType = n.ReferenceType,
        ReferenceId = n.ReferenceId,
        IsRead = n.IsRead,
        ActionUrl = n.ActionUrl,
        CreatedOn = n.CreatedOn
    };
}
