namespace CelebrationPassports.Application.Wishes.DTOs;

// "Wish" is this feature's user-facing name for a Comment left on a Chapter — matches
// the Web/Mockups redesign's "Wishes from Loved Ones" panel on the chapter/milestone
// detail page. Backed by the existing Comment entity; no schema change needed.
public class WishDto
{
    public Guid Id { get; set; }

    public Guid ChapterId { get; set; }

    public Guid UserId { get; set; }

    public string AuthorName { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
