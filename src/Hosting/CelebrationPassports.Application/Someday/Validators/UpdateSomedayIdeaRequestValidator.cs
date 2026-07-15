using CelebrationPassports.Application.Someday.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Someday.Validators;

public class UpdateSomedayIdeaRequestValidator : AbstractValidator<UpdateSomedayIdeaRequest>
{
    public UpdateSomedayIdeaRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Notes)
            .MaximumLength(2000);
    }
}
