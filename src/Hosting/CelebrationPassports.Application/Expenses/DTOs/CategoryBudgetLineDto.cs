using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Expenses.DTOs;

public class CategoryBudgetLineDto
{
    public ExpenseCategory Category { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public decimal BudgetedAmount { get; set; }

    public decimal SpentAmount { get; set; }
}
