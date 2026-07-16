namespace CelebrationPassports.Application.Imports.DTOs;

public class ImportJobDto
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    // String, not the enum — decouples the Web/API boundary from
    // CelebrationPassports.Persistence.Enums, same as every other Web-facing DTO in this
    // app (the Web project never references Persistence enums directly).
    public string Status { get; set; } = string.Empty;

    public int TotalItems { get; set; }

    public int ProcessedItems { get; set; }

    public int SkippedItems { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? CompletedOn { get; set; }
}
