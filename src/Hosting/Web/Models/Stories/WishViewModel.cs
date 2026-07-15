namespace CelebrationPassports.Web.Models.Stories;

public class WishViewModel
{
    public Guid Id { get; set; }

    public Guid ChapterId { get; set; }

    public Guid UserId { get; set; }

    public string AuthorName { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsMine { get; set; }
}
