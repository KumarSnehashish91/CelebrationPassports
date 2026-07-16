using CelebrationPassports.Web.Models.Guestbook;

namespace CelebrationPassports.Web.Interfaces;

public interface IGuestbookService
{
    Task<string?> GetShareTokenAsync(Guid chapterId);

    Task<List<GuestbookSubmissionViewModel>> GetPendingAsync(Guid chapterId);

    Task<bool> ApproveAsync(Guid submissionId);

    Task<bool> RejectAsync(Guid submissionId);

    // Anonymous — no auth token is attached for these two (BearerTokenHandler simply has
    // nothing to attach when there's no logged-in session, which is exactly right here).
    Task<GuestbookChapterInfoViewModel?> GetPublicInfoAsync(Guid chapterId, string token);

    Task<(bool Success, string? Error)> SubmitAsync(Guid chapterId, string token, string guestName, string? message, IFormFile? photo);
}
