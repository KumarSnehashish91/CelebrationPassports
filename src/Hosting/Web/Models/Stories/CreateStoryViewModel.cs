using System.ComponentModel.DataAnnotations;

namespace CelebrationPassports.Web.Models.Stories;

public class CreateStoryViewModel
{
    public Guid PassportId { get; set; }

    [Required(ErrorMessage = "Story title is required.")]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    // Simple manual entry (no live search, unlike Events' Location step) — a Story's
    // place is optional and normally AI-inferred from its Chapters/Media; this just
    // covers the "I already know where" case.
    public string? PlaceName { get; set; }

    public string? City { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}
