using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Application.Stamps.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;

namespace CelebrationPassports.Application.Stamps.Services;

public class PassportStampService : IPassportStampService
{
    private readonly IPassportRepository _passportRepository;
    private readonly IGenericRepository<PassportStamp> _stampRepository;

    public PassportStampService(
        IPassportRepository passportRepository,
        IGenericRepository<PassportStamp> stampRepository)
    {
        _passportRepository = passportRepository;
        _stampRepository = stampRepository;
    }

    public async Task<int> GetCountForUserAsync(Guid userId)
    {
        var passports = await _passportRepository.GetForUserAsync(userId);
        var passportIds = passports.Select(p => p.Id).ToList();

        if (passportIds.Count == 0)
        {
            return 0;
        }

        var stamps = await _stampRepository.FindAsync(s => passportIds.Contains(s.PassportId));

        return stamps.Count;
    }
}
