using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Expenses.DTOs;
using CelebrationPassports.Application.Expenses.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.ExpenseAPI;

[ApiController]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpPost("api/events/{eventId:guid}/expenses")]
    public async Task<IActionResult> AddExpense(Guid eventId, CreateExpenseRequest request)
    {
        var result = await _expenseService.AddExpenseAsync(User.GetUserId(), eventId, request);
        return Ok(result);
    }

    [HttpGet("api/events/{eventId:guid}/expenses")]
    public async Task<IActionResult> GetExpenses(Guid eventId)
    {
        var result = await _expenseService.GetExpensesAsync(User.GetUserId(), eventId);
        return Ok(result);
    }

    [HttpDelete("api/expenses/{id:guid}")]
    public async Task<IActionResult> DeleteExpense(Guid id)
    {
        await _expenseService.DeleteExpenseAsync(User.GetUserId(), id);
        return NoContent();
    }

    [HttpPut("api/events/{eventId:guid}/budget")]
    public async Task<IActionResult> SetCategoryBudget(Guid eventId, SetCategoryBudgetRequest request)
    {
        var result = await _expenseService.SetCategoryBudgetAsync(User.GetUserId(), eventId, request);
        return Ok(result);
    }

    [HttpGet("api/events/{eventId:guid}/budget-summary")]
    public async Task<IActionResult> GetBudgetSummary(Guid eventId)
    {
        var result = await _expenseService.GetBudgetSummaryAsync(User.GetUserId(), eventId);
        return Ok(result);
    }
}
