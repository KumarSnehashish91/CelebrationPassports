using CelebrationPassports.Application.Events.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Events.Validators;

public class AddCalendarEventRequestValidator : AbstractValidator<AddCalendarEventRequest>
{
    public AddCalendarEventRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Location)
            .MaximumLength(200);

        RuleFor(x => x.ColorTag)
            .NotEmpty()
            .MaximumLength(20);
    }
}
