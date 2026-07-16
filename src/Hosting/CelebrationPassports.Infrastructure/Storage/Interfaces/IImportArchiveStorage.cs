namespace CelebrationPassports.Infrastructure.Storage.Interfaces;

// Distinct from IFileStorageService: an import archive is transient working data the
// background processor reads back from disk to unzip, never served to a browser, so
// this deals in absolute filesystem paths rather than web-servable URLs.
public interface IImportArchiveStorage
{
    Task<string> SaveAsync(Stream content, Guid jobId, CancellationToken cancellationToken = default);

    void Delete(string archivePath);
}
