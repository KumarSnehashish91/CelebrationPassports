using CelebrationPassports.Application.Places.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Places.Validators;

public class CreatePlaceRequestValidator : AbstractValidator<CreatePlaceRequest>
{
    public CreatePlaceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Address)
            .MaximumLength(300);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.PostalCode)
            .MaximumLength(20);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Notes)
            .MaximumLength(500);
    }
}
