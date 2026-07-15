using CelebrationPassports.Application.Notifications.Interfaces;
using CelebrationPassports.Application.TimeCapsule.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;

namespace CelebrationPassports.Application.TimeCapsule.Services;

public class TimeCapsuleUnlockService : ITimeCapsuleUnlockService
{
    private readonly IGenericRepository<TimeCapsuleMessage> _messageRepository;
    private readonly IPassportRepository _passportRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public TimeCapsuleUnlockService(
        IGenericRepository<TimeCapsuleMessage> messageRepository,
        IPassportRepository passportRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _passportRepository = passportRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> UnlockDueMessagesAsync()
    {
        var now = DateTime.UtcNow;

        var due = await _messageRepository.FindAsync(m => !m.IsUnlocked && !m.IsDeleted && m.UnlockDate <= now);

        if (due.Count == 0)
        {
            return 0;
        }

        foreach (var message in due)
        {
            message.IsUnlocked = true;

            var passport = await _passportRepository.GetByIdWithPeopleAsync(message.PassportId);

            if (passport is null)
            {
                continue;
            }

            var recipientUserIds = passport.People
                .Where(p => p.UserId.HasValue)
                .Select(p => p.UserId!.Value)
                .Distinct();

            foreach (var recipientUserId in recipientUserIds)
            {
                await _notificationService.CreateAsync(
                    recipientUserId,
                    NotificationType.Reminder,
                    "A time capsule has unlocked!",
                    $"\"{message.Title}\" is ready to read.",
                    ReferenceType.TimeCapsuleMessage,
                    message.Id,
                    "/TimeCapsule/Index");
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return due.Count;
    }
}
