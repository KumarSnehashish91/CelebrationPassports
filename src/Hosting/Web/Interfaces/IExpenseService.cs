using CelebrationPassports.Web.Models.Expenses;

namespace CelebrationPassports.Web.Interfaces;

public interface IExpenseService
{
    Task<EventBudgetSummaryViewModel> GetBudgetSummaryAsync(Guid eventId);

    Task<List<ExpenseViewModel>> GetExpensesAsync(Guid eventId);

    Task<bool> AddExpenseAsync(Guid eventId, int category, string? description, decimal amount, DateOnly? spentOn);

    Task<bool> DeleteExpenseAsync(Guid expenseId);

    Task<bool> SetCategoryBudgetAsync(Guid eventId, int category, decimal budgetedAmount);
}
