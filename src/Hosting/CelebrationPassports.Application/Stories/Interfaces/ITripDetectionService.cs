namespace CelebrationPassports.Application.Stories.Interfaces;

public interface ITripDetectionService
{
    // Runs against a just-uploaded batch of (still-unattached) Media. If enough of it is
    // geo-tagged and far enough from the user's home, creates a Draft Chapter holding the
    // whole batch and a Notification pointing at it, and returns the new chapter's id.
    // Returns null (no side effects) when detection can't run at all (no home location
    // set) or doesn't trigger (nothing geo-tagged, or everything's close to home).
    Task<Guid?> DetectAsync(Guid userId, IReadOnlyList<Guid> mediaIds);
}
