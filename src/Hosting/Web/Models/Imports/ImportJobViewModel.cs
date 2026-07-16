namespace CelebrationPassports.Web.Models.Imports;

public class ImportJobViewModel
{
    public Guid Id { get; set; }

    // "Pending" | "Processing" | "Completed" | "Failed"
    public string Status { get; set; } = string.Empty;

    public int TotalItems { get; set; }

    public int ProcessedItems { get; set; }

    public int SkippedItems { get; set; }

    public string? ErrorMessage { get; set; }

    public bool IsFinished => Status is "Completed" or "Failed";
}
