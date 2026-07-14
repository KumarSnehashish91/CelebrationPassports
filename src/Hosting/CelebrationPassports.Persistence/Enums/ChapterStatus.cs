namespace CelebrationPassports.Persistence.Enums;

// Draft = auto-detected or otherwise not yet reviewed by the user; not attached to a
// Story yet (StoryId is nullable specifically to allow this). Confirmed = reviewed and
// rolled into a Story — the normal state for every manually-created Chapter, set
// immediately at creation.
public enum ChapterStatus
{
    Draft = 1,
    Confirmed = 2
}
