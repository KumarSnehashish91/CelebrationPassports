namespace CelebrationPassports.Web.Models.Sharing;

public class ChapterInvitationViewModel
{
    public Guid Id { get; set; }

    public Guid ChapterId { get; set; }

    public string ChapterTitle { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}
