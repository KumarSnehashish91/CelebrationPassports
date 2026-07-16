namespace CelebrationPassports.Application.Passports.Interfaces;

// Shared by every service that scopes its data to a Passport (Event, Story, Chapter, Media)
// so the "is this user allowed to touch this passport's data" check lives in one place.
public interface IPassportAccessGuard
{
    Task EnsureMemberAsync(Guid userId, Guid passportId);

    // Looser than EnsureMemberAsync — passes for a full passport member OR a user who
    // was granted scoped access to this one chapter only (Scoped Family Sharing,
    // feature-backlog-v1.1.md CELEBRATE #10). Use on the specific chapter-level actions a
    // scoped contributor should be able to do (view a chapter, add photos/wishes to it),
    // not on passport-wide or chapter-management actions (rename, delete, invite others).
    Task EnsureChapterAccessAsync(Guid userId, Guid chapterId);
}
