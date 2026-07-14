namespace CelebrationPassports.Web.Models.Events;

public class EventTypeOption
{
    public int Value { get; set; }

    public string Label { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    // Mirrors CelebrationPassports.Persistence.Enums.EventType — duplicated as a plain
    // list since Web no longer references Persistence directly.
    public static readonly List<EventTypeOption> All =
    [
        new() { Value = 1, Label = "Birthday", Icon = "bi-cake2" },
        new() { Value = 2, Label = "Wedding", Icon = "bi-gem" },
        new() { Value = 3, Label = "Anniversary", Icon = "bi-heart" },
        new() { Value = 4, Label = "Vacation", Icon = "bi-airplane" },
        new() { Value = 5, Label = "Baby Shower", Icon = "bi-balloon" },
        new() { Value = 6, Label = "Graduation", Icon = "bi-mortarboard" },
        new() { Value = 7, Label = "Festival", Icon = "bi-stars" },
        new() { Value = 8, Label = "Other", Icon = "bi-three-dots" }
    ];
}
