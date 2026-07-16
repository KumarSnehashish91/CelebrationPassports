namespace CelebrationPassports.Web.Models.GiftStories;

public class PrintPageViewModel
{
    public int PageNumber { get; set; }

    public string? Title { get; set; }

    public string? PhotoUrl { get; set; }

    public string? Text { get; set; }
}

public class PrintPreviewViewModel
{
    public string Format { get; set; } = string.Empty;

    public string FormatLabel { get; set; } = string.Empty;

    public string TrimSize { get; set; } = string.Empty;

    public int PageCount { get; set; }

    public decimal Price { get; set; }

    public string CoverTitle { get; set; } = string.Empty;

    public string CoverSubtitle { get; set; } = string.Empty;

    public List<PrintPageViewModel> Pages { get; set; } = [];
}
