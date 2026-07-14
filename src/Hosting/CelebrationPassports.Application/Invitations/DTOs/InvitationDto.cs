using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Invitations.DTOs;

public class InvitationDto
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string PassportTitle { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public PassportInvitationStatus Status { get; set; }
}
