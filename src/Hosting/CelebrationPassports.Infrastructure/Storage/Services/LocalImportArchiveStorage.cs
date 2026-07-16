using CelebrationPassports.Infrastructure.Storage.Interfaces;
using Microsoft.Extensions.Hosting;

namespace CelebrationPassports.Infrastructure.Storage.Services;

// Placeholder implementation, same local-disk approach as LocalFileStorageService —
// staged under App_Data (not wwwroot), since a raw import archive must never be
// reachable through static-file middleware. Replace alongside LocalFileStorageService
// when a real cloud storage provider is introduced.
public class LocalImportArchiveStorage : IImportArchiveStorage
{
    private readonly string _importsPath;

    public LocalImportArchiveStorage(IHostEnvironment environment)
    {
        _importsPath = Path.Combine(environment.ContentRootPath, "App_Data", "imports");
    }

    public async Task<string> SaveAsync(Stream content, Guid jobId, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_importsPath);

        var filePath = Path.Combine(_importsPath, $"{jobId}.zip");

        await using var fileStream = File.Create(filePath);
        await content.CopyToAsync(fileStream, cancellationToken);

        return filePath;
    }

    public void Delete(string archivePath)
    {
        if (File.Exists(archivePath))
        {
            File.Delete(archivePath);
        }
    }
}
