using CelebrationPassports.Application.Gifting.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Gifting.Validators;

public class PurchaseGiftRequestValidator : AbstractValidator<PurchaseGiftRequest>
{
    public PurchaseGiftRequestValidator()
    {
        RuleFor(x => x.RecipientName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.RecipientEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.RecipientEmail))
            .MaximumLength(256);

        RuleFor(x => x.GiftMessage)
            .MaximumLength(1000);

        RuleFor(x => x.PassportTitle)
            .MaximumLength(200);
    }
}
