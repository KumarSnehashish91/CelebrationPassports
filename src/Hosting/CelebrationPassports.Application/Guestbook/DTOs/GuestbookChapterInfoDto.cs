namespace CelebrationPassports.Application.Guestbook.DTOs;

// Deliberately minimal — this is what an anonymous, unauthenticated guest sees before
// submitting, so it carries only what's needed to confirm "yes, this is the right
// memory," not anything else about the chapter, story, or passport.
public class GuestbookChapterInfoDto
{
    public string ChapterTitle { get; set; } = string.Empty;

    public DateOnly EventDate { get; set; }
}
