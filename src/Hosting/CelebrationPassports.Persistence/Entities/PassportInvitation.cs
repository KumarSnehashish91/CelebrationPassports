using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class PassportInvitation
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public Guid InvitedBy { get; set; }

    public string Email { get; set; } = string.Empty;

    public PassportInvitationStatus Status { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual User InvitedByUser { get; set; } = null!;
}
