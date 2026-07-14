using CelebrationPassports.Application.Passports.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Passports.Validators;

public class CreatePassportRequestValidator : AbstractValidator<CreatePassportRequest>
{
    public CreatePassportRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);
    }
}
