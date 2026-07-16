using CelebrationPassports.Application.Guestbook.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Guestbook.Validators;

public class SubmitGuestbookEntryRequestValidator : AbstractValidator<SubmitGuestbookEntryRequest>
{
    public SubmitGuestbookEntryRequestValidator()
    {
        RuleFor(x => x.GuestName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Message)
            .MaximumLength(500);
    }
}
