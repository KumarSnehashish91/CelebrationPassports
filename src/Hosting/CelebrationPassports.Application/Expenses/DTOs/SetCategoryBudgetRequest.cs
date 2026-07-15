namespace CelebrationPassports.Application.Expenses.DTOs;

public class SetCategoryBudgetRequest
{
    public int Category { get; set; }

    public decimal BudgetedAmount { get; set; }
}
