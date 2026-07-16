namespace CelebrationPassports.Application.Guestbook.Interfaces;

// Same stateless-HMAC pattern as ICalendarFeedTokenService — the shareable guestbook
// link needs to work for an anonymous guest with no session, so the token is derived
// from the chapterId rather than looked up from storage.
public interface IGuestbookTokenService
{
    string GenerateToken(Guid chapterId);

    bool ValidateToken(Guid chapterId, string token);
}
