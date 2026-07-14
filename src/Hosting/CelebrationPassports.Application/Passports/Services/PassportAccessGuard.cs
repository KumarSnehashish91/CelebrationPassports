using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Persistence.Repositories.Interfaces;

namespace CelebrationPassports.Application.Passports.Services;

public class PassportAccessGuard : IPassportAccessGuard
{
    private readonly IPassportRepository _passportRepository;

    public PassportAccessGuard(IPassportRepository passportRepository)
    {
        _passportRepository = passportRepository;
    }

    public async Task EnsureMemberAsync(Guid userId, Guid passportId)
    {
        var exists = await _passportRepository.ExistsAsync(p => p.Id == passportId && !p.IsDeleted);

        if (!exists)
        {
            throw new NotFoundException("Passport not found.");
        }

        var isMember = await _passportRepository.IsMemberAsync(passportId, userId);

        if (!isMember)
        {
            throw new ForbiddenAccessException("You do not have access to this passport.");
        }
    }
}
