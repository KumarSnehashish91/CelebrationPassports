using System.ComponentModel.DataAnnotations;

namespace CelebrationPassports.Web.Models.Stories;

public class CreateChapterViewModel
{
    public Guid StoryId { get; set; }

    public string StoryTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Chapter title is required.")]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a category.")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "Chapter date is required.")]
    public DateOnly? EventDate { get; set; }

    public List<Categories.CategoryOptionViewModel> Categories { get; set; } = [];
}
