namespace CelebrationPassports.Infrastructure.Storage.Interfaces;

public interface IFileStorageService
{
    // Returns the relative URL the file is servable at (e.g. "/uploads/{name}").
    Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken = default);
}
