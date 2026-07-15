using CelebrationPassports.Application.Wishes.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Wishes.Validators;

public class CreateWishRequestValidator : AbstractValidator<CreateWishRequest>
{
    public CreateWishRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(1000);
    }
}
