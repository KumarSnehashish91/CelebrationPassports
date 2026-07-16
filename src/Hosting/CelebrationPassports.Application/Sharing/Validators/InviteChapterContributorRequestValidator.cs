using CelebrationPassports.Application.Sharing.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Sharing.Validators;

public class InviteChapterContributorRequestValidator : AbstractValidator<InviteChapterContributorRequest>
{
    public InviteChapterContributorRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
