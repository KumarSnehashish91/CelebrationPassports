using CelebrationPassports.Application.Stories.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Stories.Validators;

public class CreateChapterRequestValidator : AbstractValidator<CreateChapterRequest>
{
    public CreateChapterRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CategoryId)
            .NotEmpty();
    }
}
