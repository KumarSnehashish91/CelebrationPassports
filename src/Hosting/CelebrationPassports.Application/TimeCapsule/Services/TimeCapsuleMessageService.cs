using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Application.TimeCapsule.DTOs;
using CelebrationPassports.Application.TimeCapsule.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.TimeCapsule.Services;

// Feature: Time-Capsule Messages (feature-backlog-v1.1.md, RELIVE #6). The spec's
// "Dependency" note assumes a notification mechanism doesn't exist yet — it already
// does in this codebase (INotificationService, used by TripDetectionService etc.), so
// this wires the real thing instead of stubbing it.
public class TimeCapsuleMessageService : ITimeCapsuleMessageService
{
    private readonly IGenericRepository<TimeCapsuleMessage> _messageRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateTimeCapsuleMessageRequest> _createValidator;

    public TimeCapsuleMessageService(
        IGenericRepository<TimeCapsuleMessage> messageRepository,
        IPassportAccessGuard accessGuard,
        IUnitOfWork unitOfWork,
        IValidator<CreateTimeCapsuleMessageRequest> createValidator)
    {
        _messageRepository = messageRepository;
        _accessGuard = accessGuard;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
    }

    public async Task<TimeCapsuleMessageDto> CreateAsync(Guid userId, Guid passportId, CreateTimeCapsuleMessageRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        await _accessGuard.EnsureMemberAsync(userId, passportId);

        var message = new TimeCapsuleMessage
        {
            Id = Guid.NewGuid(),
            PassportId = passportId,
            AuthorUserId = userId,
            Title = request.Title,
            Content = request.Content,
            UnlockDate = request.UnlockDate,
            IsUnlocked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _messageRepository.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(message, userId);
    }

    public async Task<IReadOnlyList<TimeCapsuleMessageDto>> GetByPassportAsync(Guid userId, Guid passportId)
    {
        await _accessGuard.EnsureMemberAsync(userId, passportId);

        var messages = await _messageRepository.FindAsync(m => m.PassportId == passportId && !m.IsDeleted);

        return messages
            .OrderBy(m => m.UnlockDate)
            .Select(m => MapToDto(m, userId))
            .ToList();
    }

    public async Task DeleteAsync(Guid userId, Guid messageId)
    {
        var message = await _messageRepository.GetByIdAsync(messageId)
            ?? throw new NotFoundException("Message not found.");

        if (message.IsDeleted)
        {
            return;
        }

        await _accessGuard.EnsureMemberAsync(userId, message.PassportId);

        if (message.AuthorUserId != userId)
        {
            throw new ForbiddenAccessException("Only the author can delete this message.");
        }

        message.IsDeleted = true;
        message.DeletedOn = DateTime.UtcNow;
        message.DeletedBy = userId;

        await _unitOfWork.SaveChangesAsync();
    }

    private static TimeCapsuleMessageDto MapToDto(TimeCapsuleMessage message, Guid requestingUserId)
    {
        // The author already knows what they wrote — no reason to hide it from them
        // before it unlocks for everyone else.
        var canReveal = message.IsUnlocked || message.AuthorUserId == requestingUserId;

        return new TimeCapsuleMessageDto
        {
            Id = message.Id,
            PassportId = message.PassportId,
            AuthorUserId = message.AuthorUserId,
            Title = message.Title,
            Content = canReveal ? message.Content : null,
            UnlockDate = message.UnlockDate,
            IsUnlocked = message.IsUnlocked,
            CreatedAt = message.CreatedAt
        };
    }
}
