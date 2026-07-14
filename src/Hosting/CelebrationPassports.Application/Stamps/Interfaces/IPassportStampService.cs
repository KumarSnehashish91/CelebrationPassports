namespace CelebrationPassports.Application.Stamps.Interfaces;

public interface IPassportStampService
{
    Task<int> GetCountForUserAsync(Guid userId);
}
