namespace CelebrationPassports.Infrastructure.Imports.Interfaces;

public class ParsedImportMedia
{
    public required Stream Content { get; init; }

    public required string FileName { get; init; }

    public long Length { get; init; }

    public DateTime? CapturedAt { get; init; }

    public decimal? Latitude { get; init; }

    public decimal? Longitude { get; init; }
}

public interface IGooglePhotosImportParser
{
    // Cheap — a ZipArchive lists its entries as metadata on open, no stream reads.
    int CountMediaEntries(string archiveFilePath);

    // Keeps the archive open for the whole walk and opens one entry stream at a time;
    // onMediaFound must fully consume/copy the stream before returning, since the next
    // iteration reuses the same open ZipArchive.
    Task ProcessArchiveAsync(
        string archiveFilePath,
        Func<ParsedImportMedia, Task> onMediaFound,
        CancellationToken cancellationToken);
}
