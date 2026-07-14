using CelebrationPassports.Web.Models.Invitations;

namespace CelebrationPassports.Web.Interfaces;

public interface IInvitationService
{
    Task<List<InvitationViewModel>> GetPendingAsync();

    Task<List<InvitationViewModel>> GetByPassportAsync(Guid passportId);

    Task<bool> InviteAsync(Guid passportId, string email);

    Task<bool> AcceptAsync(Guid invitationId);

    Task<bool> DeclineAsync(Guid invitationId);
}
