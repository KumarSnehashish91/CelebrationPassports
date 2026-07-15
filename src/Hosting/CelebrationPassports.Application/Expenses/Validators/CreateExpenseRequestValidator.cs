using CelebrationPassports.Application.Expenses.DTOs;
using CelebrationPassports.Persistence.Enums;
using FluentValidation;

namespace CelebrationPassports.Application.Expenses.Validators;

public class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequest>
{
    public CreateExpenseRequestValidator()
    {
        RuleFor(x => x.Category)
            .Must(c => Enum.IsDefined(typeof(ExpenseCategory), c))
            .WithMessage("Please choose a valid expense category.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Description)
            .MaximumLength(300);
    }
}
