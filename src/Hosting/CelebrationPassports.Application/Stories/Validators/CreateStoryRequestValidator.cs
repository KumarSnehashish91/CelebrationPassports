using CelebrationPassports.Application.Stories.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Stories.Validators;

public class CreateStoryRequestValidator : AbstractValidator<CreateStoryRequest>
{
    public CreateStoryRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}
