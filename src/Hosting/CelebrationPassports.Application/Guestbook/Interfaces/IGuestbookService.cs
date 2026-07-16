using CelebrationPassports.Application.Guestbook.DTOs;
using CelebrationPassports.Application.Media.DTOs;

namespace CelebrationPassports.Application.Guestbook.Interfaces;

public interface IGuestbookService
{
    // Authenticated — any member of the chapter's passport can fetch/share the link.
    Task<string> GetShareTokenAsync(Guid userId, Guid chapterId);

    // Anonymous — what a guest sees before submitting.
    Task<GuestbookChapterInfoDto> GetPublicInfoAsync(Guid chapterId, string token);

    // Anonymous — the actual submission. Always lands as Pending; never touches the
    // chapter's real media/wishes until a member approves it.
    Task SubmitAsync(Guid chapterId, string token, SubmitGuestbookEntryRequest request, FileUploadRequest? photo);

    Task<IReadOnlyList<GuestbookSubmissionDto>> GetPendingAsync(Guid userId, Guid chapterId);

    Task ApproveAsync(Guid userId, Guid submissionId);

    Task RejectAsync(Guid userId, Guid submissionId);
}
