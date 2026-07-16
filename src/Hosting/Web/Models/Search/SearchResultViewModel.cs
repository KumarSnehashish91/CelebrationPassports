namespace CelebrationPassports.Web.Models.Search;

public class SearchResultViewModel
{
    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Subtitle { get; set; }

    public string Url { get; set; } = string.Empty;

    public string Icon { get; set; } = "bi-search";
}
