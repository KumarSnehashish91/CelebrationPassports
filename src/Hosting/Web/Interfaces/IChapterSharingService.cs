using CelebrationPassports.Web.Models.Sharing;

namespace CelebrationPassports.Web.Interfaces;

public interface IChapterSharingService
{
    Task<bool> InviteAsync(Guid chapterId, string email);

    Task<List<ChapterInvitationViewModel>> GetPendingForMeAsync();

    Task<List<ChapterInvitationViewModel>> GetByChapterAsync(Guid chapterId);

    Task<List<ChapterContributorViewModel>> GetContributorsAsync(Guid chapterId);

    // Returns the ChapterId to send the user to, or null if the accept call failed.
    Task<Guid?> AcceptAsync(Guid invitationId);

    Task<bool> DeclineAsync(Guid invitationId);

    Task<bool> RemoveContributorAsync(Guid contributorId);
}
