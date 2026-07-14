using System.ComponentModel.DataAnnotations;

namespace CelebrationPassports.Web.Models.Stories;

public class ConfirmChapterViewModel
{
    public Guid ChapterId { get; set; }

    // Carried through unchanged from the draft — this form doesn't offer place editing
    // (the AI-detected place, or lack of one, stays as-is; edit it from the Story later).
    public Guid? PlaceId { get; set; }

    [Required(ErrorMessage = "Chapter title is required.")]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a category.")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "Chapter date is required.")]
    public DateOnly? EventDate { get; set; }

    // "existing-{guid}" or "new" — parsed in the controller; keeps the form to one
    // radio-style select instead of two separately-required fields.
    public string StoryChoice { get; set; } = "new";

    public string? NewStoryTitle { get; set; }

    public List<Categories.CategoryOptionViewModel> Categories { get; set; } = [];

    public List<StoryListItemViewModel> ExistingStories { get; set; } = [];
}
