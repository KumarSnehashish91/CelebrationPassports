using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Application.Sharing.DTOs;
using CelebrationPassports.Application.Sharing.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.Sharing.Services;

public class ChapterSharingService : IChapterSharingService
{
    private readonly IGenericRepository<ChapterInvitation> _invitationRepository;
    private readonly IGenericRepository<ChapterContributor> _contributorRepository;
    private readonly IGenericRepository<Chapter> _chapterRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<InviteChapterContributorRequest> _inviteValidator;

    public ChapterSharingService(
        IGenericRepository<ChapterInvitation> invitationRepository,
        IGenericRepository<ChapterContributor> contributorRepository,
        IGenericRepository<Chapter> chapterRepository,
        IUserProfileRepository userProfileRepository,
        IPassportAccessGuard accessGuard,
        IUnitOfWork unitOfWork,
        IValidator<InviteChapterContributorRequest> inviteValidator)
    {
        _invitationRepository = invitationRepository;
        _contributorRepository = contributorRepository;
        _chapterRepository = chapterRepository;
        _userProfileRepository = userProfileRepository;
        _accessGuard = accessGuard;
        _unitOfWork = unitOfWork;
        _inviteValidator = inviteValidator;
    }

    public async Task<ChapterInvitationDto> InviteAsync(Guid userId, Guid chapterId, InviteChapterContributorRequest request)
    {
        await _inviteValidator.ValidateAndThrowAsync(request);

        var chapter = await _chapterRepository.GetByIdAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        // Inviting a scoped contributor is a full-member-only action — a contributor
        // themselves can't extend access to others.
        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        var alreadyPending = await _invitationRepository.ExistsAsync(i =>
            i.ChapterId == chapterId && i.Email == request.Email && i.Status == ChapterInvitationStatus.Pending);

        if (alreadyPending)
        {
            throw new ValidationException($"An invitation to '{request.Email}' for this chapter is already pending.");
        }

        var invitation = new ChapterInvitation
        {
            Id = Guid.NewGuid(),
            ChapterId = chapterId,
            InvitedBy = userId,
            Email = request.Email,
            Status = ChapterInvitationStatus.Pending
        };

        await _invitationRepository.AddAsync(invitation);
        await _unitOfWork.SaveChangesAsync();

        return new ChapterInvitationDto
        {
            Id = invitation.Id,
            ChapterId = chapterId,
            ChapterTitle = chapter.Title,
            Email = invitation.Email,
            Status = invitation.Status
        };
    }

    public async Task<IReadOnlyList<ChapterInvitationDto>> GetPendingForMeAsync(Guid userId, string email)
    {
        var invitations = await _invitationRepository.FindAsync(i =>
            i.Email == email && i.Status == ChapterInvitationStatus.Pending);

        var result = new List<ChapterInvitationDto>();

        foreach (var invitation in invitations)
        {
            var chapter = await _chapterRepository.GetByIdAsync(invitation.ChapterId);

            result.Add(new ChapterInvitationDto
            {
                Id = invitation.Id,
                ChapterId = invitation.ChapterId,
                ChapterTitle = chapter?.Title ?? "Unknown chapter",
                Email = invitation.Email,
                Status = invitation.Status
            });
        }

        return result;
    }

    public async Task<IReadOnlyList<ChapterInvitationDto>> GetByChapterAsync(Guid userId, Guid chapterId)
    {
        var chapter = await _chapterRepository.GetByIdAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        var invitations = await _invitationRepository.FindAsync(i => i.ChapterId == chapterId);

        return invitations.Select(i => new ChapterInvitationDto
        {
            Id = i.Id,
            ChapterId = i.ChapterId,
            ChapterTitle = chapter.Title,
            Email = i.Email,
            Status = i.Status
        }).ToList();
    }

    public async Task<IReadOnlyList<ChapterContributorDto>> GetContributorsAsync(Guid userId, Guid chapterId)
    {
        var chapter = await _chapterRepository.GetByIdAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        var contributors = await _contributorRepository.FindAsync(c => c.ChapterId == chapterId);

        var result = new List<ChapterContributorDto>();

        foreach (var contributor in contributors)
        {
            var name = await ResolveNameAsync(contributor.UserId);

            result.Add(new ChapterContributorDto
            {
                Id = contributor.Id,
                ChapterId = contributor.ChapterId,
                UserId = contributor.UserId,
                Name = name,
                CreatedAt = contributor.CreatedAt
            });
        }

        return result;
    }

    public async Task<Guid> AcceptAsync(Guid userId, Guid invitationId, string email)
    {
        var invitation = await GetOwnPendingInvitationAsync(invitationId, email);

        invitation.Status = ChapterInvitationStatus.Accepted;

        var alreadyContributor = await _contributorRepository.ExistsAsync(c =>
            c.ChapterId == invitation.ChapterId && c.UserId == userId);

        if (!alreadyContributor)
        {
            var contributor = new ChapterContributor
            {
                Id = Guid.NewGuid(),
                ChapterId = invitation.ChapterId,
                UserId = userId,
                InvitedBy = invitation.InvitedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _contributorRepository.AddAsync(contributor);
        }

        await _unitOfWork.SaveChangesAsync();

        return invitation.ChapterId;
    }

    public async Task DeclineAsync(Guid userId, Guid invitationId, string email)
    {
        var invitation = await GetOwnPendingInvitationAsync(invitationId, email);

        invitation.Status = ChapterInvitationStatus.Declined;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveContributorAsync(Guid userId, Guid contributorId)
    {
        var contributor = await _contributorRepository.GetByIdAsync(contributorId)
            ?? throw new NotFoundException("Contributor not found.");

        var chapter = await _chapterRepository.GetByIdAsync(contributor.ChapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        _contributorRepository.Remove(contributor);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<ChapterInvitation> GetOwnPendingInvitationAsync(Guid invitationId, string email)
    {
        var invitation = await _invitationRepository.GetByIdAsync(invitationId)
            ?? throw new NotFoundException("Invitation not found.");

        if (!string.Equals(invitation.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenAccessException("This invitation was not sent to you.");
        }

        if (invitation.Status != ChapterInvitationStatus.Pending)
        {
            throw new ValidationException("This invitation is no longer pending.");
        }

        return invitation;
    }

    private async Task<string> ResolveNameAsync(Guid userId)
    {
        var profile = await _userProfileRepository.GetUserProfileByIdAsync(userId);

        if (profile is null)
        {
            return "Someone";
        }

        return string.IsNullOrWhiteSpace(profile.DisplayName)
            ? $"{profile.FirstName} {profile.LastName}".Trim()
            : profile.DisplayName;
    }
}
