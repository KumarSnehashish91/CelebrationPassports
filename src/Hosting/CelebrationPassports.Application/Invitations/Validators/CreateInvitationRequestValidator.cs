using CelebrationPassports.Application.Invitations.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Invitations.Validators;

public class CreateInvitationRequestValidator : AbstractValidator<CreateInvitationRequest>
{
    public CreateInvitationRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
