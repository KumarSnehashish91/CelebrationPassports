using CelebrationPassports.Application.Someday.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Someday.Validators;

public class CreateSomedayIdeaRequestValidator : AbstractValidator<CreateSomedayIdeaRequest>
{
    public CreateSomedayIdeaRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Notes)
            .MaximumLength(2000);
    }
}
