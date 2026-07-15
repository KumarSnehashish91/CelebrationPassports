using CelebrationPassports.Application.TimeCapsule.DTOs;

namespace CelebrationPassports.Application.TimeCapsule.Interfaces;

public interface ITimeCapsuleMessageService
{
    Task<TimeCapsuleMessageDto> CreateAsync(Guid userId, Guid passportId, CreateTimeCapsuleMessageRequest request);

    Task<IReadOnlyList<TimeCapsuleMessageDto>> GetByPassportAsync(Guid userId, Guid passportId);

    Task DeleteAsync(Guid userId, Guid messageId);
}
