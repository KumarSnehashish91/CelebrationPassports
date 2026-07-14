using CelebrationPassports.Application.Events.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Events.Validators;

public class UpdateEventRequestValidator : AbstractValidator<UpdateEventRequest>
{
    public UpdateEventRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Notes)
            .MaximumLength(2000);

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue);
    }
}
