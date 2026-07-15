using CelebrationPassports.Application.TimeCapsule.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.TimeCapsule.Validators;

public class CreateTimeCapsuleMessageRequestValidator : AbstractValidator<CreateTimeCapsuleMessageRequest>
{
    public CreateTimeCapsuleMessageRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(5000);

        RuleFor(x => x.UnlockDate)
            .GreaterThan(_ => DateTime.UtcNow)
            .WithMessage("Unlock date must be in the future — a time capsule that opens immediately isn't one.");
    }
}
