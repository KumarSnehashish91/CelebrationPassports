using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Expenses.DTOs;
using CelebrationPassports.Application.Expenses.Interfaces;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.Expenses.Services;

// Budget vs. spend is intentionally two separate tables (ExpenseCategoryBudget for the
// amount the user set aside, Expense for the individual line items actually logged) so
// the "spent" side is always a live sum over real entries rather than a cached total that
// could drift. Categories are the fixed set in ExpenseCategory, matching the mockup's
// Trip Detail budget card (Stay / Travel / Food / Activities / Other) — no free-text
// categories, so summaries stay simple to render as fixed rows.
public class ExpenseService : IExpenseService
{
    private readonly IGenericRepository<Expense> _expenseRepository;
    private readonly IGenericRepository<ExpenseCategoryBudget> _budgetRepository;
    private readonly IGenericRepository<Event> _eventRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateExpenseRequest> _createExpenseValidator;
    private readonly IValidator<SetCategoryBudgetRequest> _setBudgetValidator;

    public ExpenseService(
        IGenericRepository<Expense> expenseRepository,
        IGenericRepository<ExpenseCategoryBudget> budgetRepository,
        IGenericRepository<Event> eventRepository,
        IUserProfileRepository userProfileRepository,
        IPassportAccessGuard accessGuard,
        IUnitOfWork unitOfWork,
        IValidator<CreateExpenseRequest> createExpenseValidator,
        IValidator<SetCategoryBudgetRequest> setBudgetValidator)
    {
        _expenseRepository = expenseRepository;
        _budgetRepository = budgetRepository;
        _eventRepository = eventRepository;
        _userProfileRepository = userProfileRepository;
        _accessGuard = accessGuard;
        _unitOfWork = unitOfWork;
        _createExpenseValidator = createExpenseValidator;
        _setBudgetValidator = setBudgetValidator;
    }

    public async Task<ExpenseDto> AddExpenseAsync(Guid userId, Guid eventId, CreateExpenseRequest request)
    {
        await _createExpenseValidator.ValidateAndThrowAsync(request);

        var @event = await _eventRepository.GetByIdAsync(eventId)
            ?? throw new NotFoundException("Event not found.");

        await _accessGuard.EnsureMemberAsync(userId, @event.PassportId);

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Category = (ExpenseCategory)request.Category,
            Description = request.Description,
            Amount = request.Amount,
            SpentOn = request.SpentOn ?? DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _expenseRepository.AddAsync(expense);
        await _unitOfWork.SaveChangesAsync();

        var authorName = await ResolveUserNameAsync(userId);

        return MapToDto(expense, authorName);
    }

    public async Task<IReadOnlyList<ExpenseDto>> GetExpensesAsync(Guid userId, Guid eventId)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId)
            ?? throw new NotFoundException("Event not found.");

        await _accessGuard.EnsureMemberAsync(userId, @event.PassportId);

        var expenses = await _expenseRepository.FindAsync(e => e.EventId == eventId && !e.IsDeleted);

        var result = new List<ExpenseDto>();

        foreach (var expense in expenses.OrderByDescending(e => e.SpentOn).ThenByDescending(e => e.CreatedAt))
        {
            var authorName = await ResolveUserNameAsync(expense.CreatedByUserId);
            result.Add(MapToDto(expense, authorName));
        }

        return result;
    }

    public async Task DeleteExpenseAsync(Guid userId, Guid expenseId)
    {
        var expense = await _expenseRepository.GetByIdAsync(expenseId)
            ?? throw new NotFoundException("Expense not found.");

        if (expense.IsDeleted)
        {
            return;
        }

        if (expense.CreatedByUserId != userId)
        {
            throw new ForbiddenAccessException("Only the person who logged this expense can delete it.");
        }

        expense.IsDeleted = true;
        expense.DeletedOn = DateTime.UtcNow;
        expense.DeletedBy = userId;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<CategoryBudgetLineDto> SetCategoryBudgetAsync(Guid userId, Guid eventId, SetCategoryBudgetRequest request)
    {
        await _setBudgetValidator.ValidateAndThrowAsync(request);

        var @event = await _eventRepository.GetByIdAsync(eventId)
            ?? throw new NotFoundException("Event not found.");

        await _accessGuard.EnsureMemberAsync(userId, @event.PassportId);

        var category = (ExpenseCategory)request.Category;

        var existing = (await _budgetRepository.FindAsync(b => b.EventId == eventId && b.Category == category))
            .FirstOrDefault();

        if (existing is null)
        {
            existing = new ExpenseCategoryBudget
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                Category = category,
                BudgetedAmount = request.BudgetedAmount,
                CreatedAt = DateTime.UtcNow
            };

            await _budgetRepository.AddAsync(existing);
        }
        else
        {
            existing.BudgetedAmount = request.BudgetedAmount;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();

        var spent = (await _expenseRepository.FindAsync(e => e.EventId == eventId && !e.IsDeleted && e.Category == category))
            .Sum(e => e.Amount);

        return new CategoryBudgetLineDto
        {
            Category = category,
            CategoryName = category.ToString(),
            BudgetedAmount = existing.BudgetedAmount,
            SpentAmount = spent
        };
    }

    public async Task<EventBudgetSummaryDto> GetBudgetSummaryAsync(Guid userId, Guid eventId)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId)
            ?? throw new NotFoundException("Event not found.");

        await _accessGuard.EnsureMemberAsync(userId, @event.PassportId);

        var budgets = await _budgetRepository.FindAsync(b => b.EventId == eventId);
        var expenses = await _expenseRepository.FindAsync(e => e.EventId == eventId && !e.IsDeleted);

        var spentByCategory = expenses
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        var budgetByCategory = budgets.ToDictionary(b => b.Category, b => b.BudgetedAmount);

        var categories = Enum.GetValues<ExpenseCategory>()
            .Where(c => budgetByCategory.ContainsKey(c) || spentByCategory.ContainsKey(c))
            .Select(c => new CategoryBudgetLineDto
            {
                Category = c,
                CategoryName = c.ToString(),
                BudgetedAmount = budgetByCategory.GetValueOrDefault(c),
                SpentAmount = spentByCategory.GetValueOrDefault(c)
            })
            .OrderBy(c => (int)c.Category)
            .ToList();

        return new EventBudgetSummaryDto
        {
            EventId = eventId,
            Categories = categories,
            TotalBudgeted = categories.Sum(c => c.BudgetedAmount),
            TotalSpent = categories.Sum(c => c.SpentAmount)
        };
    }

    private async Task<string> ResolveUserNameAsync(Guid userId)
    {
        var profile = await _userProfileRepository.GetUserProfileByIdAsync(userId);

        if (profile is null)
        {
            return "Someone";
        }

        return string.IsNullOrWhiteSpace(profile.DisplayName)
            ? $"{profile.FirstName} {profile.LastName}".Trim()
            : profile.DisplayName;
    }

    private static ExpenseDto MapToDto(Expense expense, string authorName) => new()
    {
        Id = expense.Id,
        EventId = expense.EventId,
        Category = expense.Category,
        CategoryName = expense.Category.ToString(),
        Description = expense.Description,
        Amount = expense.Amount,
        SpentOn = expense.SpentOn,
        CreatedByUserId = expense.CreatedByUserId,
        CreatedByName = authorName,
        CreatedAt = expense.CreatedAt
    };
}
