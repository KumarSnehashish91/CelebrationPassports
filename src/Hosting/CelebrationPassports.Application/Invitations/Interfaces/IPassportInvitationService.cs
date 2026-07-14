using CelebrationPassports.Application.Invitations.DTOs;

namespace CelebrationPassports.Application.Invitations.Interfaces;

public interface IPassportInvitationService
{
    Task<InvitationDto> InviteAsync(Guid userId, Guid passportId, CreateInvitationRequest request);

    Task<IReadOnlyList<InvitationDto>> GetPendingForMeAsync(Guid userId, string email);

    Task<IReadOnlyList<InvitationDto>> GetByPassportAsync(Guid userId, Guid passportId);

    Task AcceptAsync(Guid userId, Guid invitationId, string email);

    Task DeclineAsync(Guid userId, Guid invitationId, string email);
}
