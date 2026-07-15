namespace CelebrationPassports.Web.Models.Expenses;

public class ExpenseCategoryOption
{
    public int Value { get; set; }

    public string Label { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    // Mirrors CelebrationPassports.Persistence.Enums.ExpenseCategory — duplicated as a
    // plain list since Web no longer references Persistence directly.
    public static readonly List<ExpenseCategoryOption> All =
    [
        new() { Value = 1, Label = "Stay", Icon = "bi-house-door" },
        new() { Value = 2, Label = "Travel", Icon = "bi-airplane" },
        new() { Value = 3, Label = "Food", Icon = "bi-cup-straw" },
        new() { Value = 4, Label = "Activities", Icon = "bi-ticket-perforated" },
        new() { Value = 5, Label = "Other", Icon = "bi-three-dots" }
    ];
}
