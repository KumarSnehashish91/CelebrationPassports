using CelebrationPassports.Web.Models.TimeCapsule;

namespace CelebrationPassports.Web.Interfaces;

public interface ITimeCapsuleService
{
    Task<List<TimeCapsuleMessageViewModel>> GetByPassportAsync(Guid passportId);

    Task<bool> CreateAsync(Guid passportId, string title, string content, DateTime unlockDate);

    Task<bool> DeleteAsync(Guid id);
}
