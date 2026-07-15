using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Expenses;

namespace CelebrationPassports.Web.Services;

public class ExpenseService : IExpenseService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public ExpenseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<EventBudgetSummaryViewModel> GetBudgetSummaryAsync(Guid eventId)
    {
        var response = await _httpClient.GetAsync($"api/events/{eventId}/budget-summary");

        if (!response.IsSuccessStatusCode)
        {
            return new EventBudgetSummaryViewModel { EventId = eventId };
        }

        var body = await response.Content.ReadFromJsonAsync<SummaryBody>(JsonOptions);

        if (body is null)
        {
            return new EventBudgetSummaryViewModel { EventId = eventId };
        }

        return new EventBudgetSummaryViewModel
        {
            EventId = body.EventId,
            TotalBudgeted = body.TotalBudgeted,
            TotalSpent = body.TotalSpent,
            Categories = body.Categories.Select(c => new CategoryBudgetViewModel
            {
                Category = c.Category,
                CategoryName = c.CategoryName,
                BudgetedAmount = c.BudgetedAmount,
                SpentAmount = c.SpentAmount
            }).ToList()
        };
    }

    public async Task<List<ExpenseViewModel>> GetExpensesAsync(Guid eventId)
    {
        var response = await _httpClient.GetAsync($"api/events/{eventId}/expenses");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<ExpenseBody>>(JsonOptions);

        return body?.Select(e => new ExpenseViewModel
        {
            Id = e.Id,
            EventId = e.EventId,
            Category = e.Category,
            CategoryName = e.CategoryName,
            Description = e.Description,
            Amount = e.Amount,
            SpentOn = e.SpentOn,
            CreatedByUserId = e.CreatedByUserId,
            CreatedByName = e.CreatedByName,
            CreatedAt = e.CreatedAt
        }).ToList() ?? [];
    }

    public async Task<bool> AddExpenseAsync(Guid eventId, int category, string? description, decimal amount, DateOnly? spentOn)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/events/{eventId}/expenses", new
        {
            category,
            description,
            amount,
            spentOn
        });

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteExpenseAsync(Guid expenseId)
    {
        var response = await _httpClient.DeleteAsync($"api/expenses/{expenseId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SetCategoryBudgetAsync(Guid eventId, int category, decimal budgetedAmount)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/events/{eventId}/budget", new
        {
            category,
            budgetedAmount
        });

        return response.IsSuccessStatusCode;
    }

    private sealed class SummaryBody
    {
        public Guid EventId { get; set; }
        public List<CategoryBody> Categories { get; set; } = [];
        public decimal TotalBudgeted { get; set; }
        public decimal TotalSpent { get; set; }
    }

    private sealed class CategoryBody
    {
        public int Category { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal BudgetedAmount { get; set; }
        public decimal SpentAmount { get; set; }
    }

    private sealed class ExpenseBody
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public int Category { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateOnly SpentOn { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
