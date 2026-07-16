using CelebrationPassports.Application.Sharing.DTOs;

namespace CelebrationPassports.Application.Sharing.Interfaces;

// Scoped Family Sharing (feature-backlog-v1.1.md, CELEBRATE #10) — lets a full passport
// member invite someone to a single chapter without making them a full passport member.
public interface IChapterSharingService
{
    Task<ChapterInvitationDto> InviteAsync(Guid userId, Guid chapterId, InviteChapterContributorRequest request);

    Task<IReadOnlyList<ChapterInvitationDto>> GetPendingForMeAsync(Guid userId, string email);

    Task<IReadOnlyList<ChapterInvitationDto>> GetByChapterAsync(Guid userId, Guid chapterId);

    Task<IReadOnlyList<ChapterContributorDto>> GetContributorsAsync(Guid userId, Guid chapterId);

    // Returns the ChapterId that was just granted, so the caller can send the user
    // straight to it instead of a generic "nothing to see" landing page.
    Task<Guid> AcceptAsync(Guid userId, Guid invitationId, string email);

    Task DeclineAsync(Guid userId, Guid invitationId, string email);

    Task RemoveContributorAsync(Guid userId, Guid contributorId);
}
