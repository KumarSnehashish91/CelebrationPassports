namespace CelebrationPassports.Application.GiftStories.DTOs;

public class PrintPageDto
{
    public int PageNumber { get; set; }

    public string? Title { get; set; }

    public string? PhotoUrl { get; set; }

    public string? Text { get; set; }
}

public class PrintPreviewDto
{
    // "LifeBook" | "PassportEdition"
    public string Format { get; set; } = string.Empty;

    public string FormatLabel { get; set; } = string.Empty;

    public string TrimSize { get; set; } = string.Empty;

    public int PageCount { get; set; }

    public decimal Price { get; set; }

    public string CoverTitle { get; set; } = string.Empty;

    public string CoverSubtitle { get; set; } = string.Empty;

    public List<PrintPageDto> Pages { get; set; } = [];
}
