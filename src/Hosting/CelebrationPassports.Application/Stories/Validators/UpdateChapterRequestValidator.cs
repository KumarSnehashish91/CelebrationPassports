using CelebrationPassports.Application.Stories.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Stories.Validators;

public class UpdateChapterRequestValidator : AbstractValidator<UpdateChapterRequest>
{
    public UpdateChapterRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
