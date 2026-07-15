using CelebrationPassports.Application.Expenses.DTOs;
using CelebrationPassports.Persistence.Enums;
using FluentValidation;

namespace CelebrationPassports.Application.Expenses.Validators;

public class SetCategoryBudgetRequestValidator : AbstractValidator<SetCategoryBudgetRequest>
{
    public SetCategoryBudgetRequestValidator()
    {
        RuleFor(x => x.Category)
            .Must(c => Enum.IsDefined(typeof(ExpenseCategory), c))
            .WithMessage("Please choose a valid expense category.");

        RuleFor(x => x.BudgetedAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Budget can't be negative.");
    }
}
