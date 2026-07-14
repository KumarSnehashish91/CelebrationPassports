namespace CelebrationPassports.Application.Passports.Interfaces;

// Shared by every service that scopes its data to a Passport (Event, Story, Chapter, Media)
// so the "is this user allowed to touch this passport's data" check lives in one place.
public interface IPassportAccessGuard
{
    Task EnsureMemberAsync(Guid userId, Guid passportId);
}
