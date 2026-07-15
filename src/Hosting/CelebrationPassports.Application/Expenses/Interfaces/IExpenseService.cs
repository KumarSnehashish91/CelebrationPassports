using CelebrationPassports.Application.Expenses.DTOs;

namespace CelebrationPassports.Application.Expenses.Interfaces;

public interface IExpenseService
{
    Task<ExpenseDto> AddExpenseAsync(Guid userId, Guid eventId, CreateExpenseRequest request);

    Task<IReadOnlyList<ExpenseDto>> GetExpensesAsync(Guid userId, Guid eventId);

    Task DeleteExpenseAsync(Guid userId, Guid expenseId);

    Task<CategoryBudgetLineDto> SetCategoryBudgetAsync(Guid userId, Guid eventId, SetCategoryBudgetRequest request);

    Task<EventBudgetSummaryDto> GetBudgetSummaryAsync(Guid userId, Guid eventId);
}
