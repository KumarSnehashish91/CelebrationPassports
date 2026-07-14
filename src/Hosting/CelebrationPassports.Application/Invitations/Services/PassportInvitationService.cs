using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Invitations.DTOs;
using CelebrationPassports.Application.Invitations.Interfaces;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.Invitations.Services;

public class PassportInvitationService : IPassportInvitationService
{
    private readonly IPassportInvitationRepository _invitationRepository;
    private readonly IPassportRepository _passportRepository;
    private readonly IGenericRepository<PassportPerson> _passportPersonRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateInvitationRequest> _createValidator;

    public PassportInvitationService(
        IPassportInvitationRepository invitationRepository,
        IPassportRepository passportRepository,
        IGenericRepository<PassportPerson> passportPersonRepository,
        IUserProfileRepository userProfileRepository,
        IPassportAccessGuard accessGuard,
        IUnitOfWork unitOfWork,
        IValidator<CreateInvitationRequest> createValidator)
    {
        _invitationRepository = invitationRepository;
        _passportRepository = passportRepository;
        _passportPersonRepository = passportPersonRepository;
        _userProfileRepository = userProfileRepository;
        _accessGuard = accessGuard;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
    }

    public async Task<InvitationDto> InviteAsync(Guid userId, Guid passportId, CreateInvitationRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        await _accessGuard.EnsureMemberAsync(userId, passportId);

        if (await _invitationRepository.ExistsPendingAsync(passportId, request.Email))
        {
            throw new ValidationException($"An invitation to '{request.Email}' for this passport is already pending.");
        }

        var passport = await _passportRepository.GetByIdAsync(passportId)
            ?? throw new NotFoundException("Passport not found.");

        var invitation = new PassportInvitation
        {
            Id = Guid.NewGuid(),
            PassportId = passportId,
            InvitedBy = userId,
            Email = request.Email,
            Status = PassportInvitationStatus.Pending
        };

        await _invitationRepository.AddAsync(invitation);
        await _unitOfWork.SaveChangesAsync();

        return new InvitationDto
        {
            Id = invitation.Id,
            PassportId = passportId,
            PassportTitle = passport.Title,
            Email = invitation.Email,
            Status = invitation.Status
        };
    }

    private static InvitationDto MapToDto(PassportInvitation invitation) => new()
    {
        Id = invitation.Id,
        PassportId = invitation.PassportId,
        PassportTitle = invitation.Passport.Title,
        Email = invitation.Email,
        Status = invitation.Status
    };

    public async Task<IReadOnlyList<InvitationDto>> GetPendingForMeAsync(Guid userId, string email)
    {
        var invitations = await _invitationRepository.GetPendingForEmailAsync(email);

        return invitations.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<InvitationDto>> GetByPassportAsync(Guid userId, Guid passportId)
    {
        await _accessGuard.EnsureMemberAsync(userId, passportId);

        var invitations = await _invitationRepository.GetByPassportAsync(passportId);

        return invitations.Select(MapToDto).ToList();
    }

    public async Task AcceptAsync(Guid userId, Guid invitationId, string email)
    {
        var invitation = await GetOwnPendingInvitationAsync(invitationId, email);

        invitation.Status = PassportInvitationStatus.Accepted;

        var profile = await _userProfileRepository.GetUserProfileByIdAsync(userId);
        var name = profile is null
            ? "Unknown"
            : string.IsNullOrWhiteSpace(profile.DisplayName)
                ? $"{profile.FirstName} {profile.LastName}".Trim()
                : profile.DisplayName;

        var person = new PassportPerson
        {
            Id = Guid.NewGuid(),
            PassportId = invitation.PassportId,
            UserId = userId,
            Name = name,
            Role = PassportPersonRole.Contributor
        };

        await _passportPersonRepository.AddAsync(person);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeclineAsync(Guid userId, Guid invitationId, string email)
    {
        var invitation = await GetOwnPendingInvitationAsync(invitationId, email);

        invitation.Status = PassportInvitationStatus.Declined;

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<PassportInvitation> GetOwnPendingInvitationAsync(Guid invitationId, string email)
    {
        var invitation = await _invitationRepository.GetByIdAsync(invitationId)
            ?? throw new NotFoundException("Invitation not found.");

        if (!string.Equals(invitation.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenAccessException("This invitation was not sent to you.");
        }

        if (invitation.Status != PassportInvitationStatus.Pending)
        {
            throw new ValidationException("This invitation is no longer pending.");
        }

        return invitation;
    }
}
