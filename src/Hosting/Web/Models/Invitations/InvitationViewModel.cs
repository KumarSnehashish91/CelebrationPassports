namespace CelebrationPassports.Web.Models.Invitations;

public class InvitationViewModel
{
    public Guid Id { get; set; }

    public string PassportTitle { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}
