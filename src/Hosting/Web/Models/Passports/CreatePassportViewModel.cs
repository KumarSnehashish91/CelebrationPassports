using System.ComponentModel.DataAnnotations;

namespace CelebrationPassports.Web.Models.Passports;

public class CreatePassportViewModel
{
    [Required(ErrorMessage = "Title is required.")]
    [Display(Name = "Passport Title")]
    public string Title { get; set; } = string.Empty;
}
