using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;

namespace CelebrationPassports.Application.Passports.Services;

public class PassportAccessGuard : IPassportAccessGuard
{
    private readonly IPassportRepository _passportRepository;
    private readonly IGenericRepository<Chapter> _chapterRepository;
    private readonly IGenericRepository<ChapterContributor> _chapterContributorRepository;

    public PassportAccessGuard(
        IPassportRepository passportRepository,
        IGenericRepository<Chapter> chapterRepository,
        IGenericRepository<ChapterContributor> chapterContributorRepository)
    {
        _passportRepository = passportRepository;
        _chapterRepository = chapterRepository;
        _chapterContributorRepository = chapterContributorRepository;
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

    public async Task EnsureChapterAccessAsync(Guid userId, Guid chapterId)
    {
        var chapter = await _chapterRepository.GetByIdAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        var isFullMember = await _passportRepository.IsMemberAsync(chapter.PassportId, userId);

        if (isFullMember)
        {
            return;
        }

        var isScopedContributor = await _chapterContributorRepository
            .ExistsAsync(c => c.ChapterId == chapterId && c.UserId == userId);

        if (!isScopedContributor)
        {
            throw new ForbiddenAccessException("You do not have access to this chapter.");
        }
    }
}
