namespace CelebrationPassports.Application.Calendar.Interfaces;

// Calendar apps poll a subscribed feed URL directly — they can't send a Bearer JWT, so
// the feed URL itself carries a deterministic, unguessable token instead. No new
// storage: the token is derived (HMAC) from the userId, not persisted anywhere, so
// there's nothing to add to the schema and nothing to look up to validate it.
public interface ICalendarFeedTokenService
{
    string GenerateToken(Guid userId);

    bool ValidateToken(Guid userId, string token);
}
