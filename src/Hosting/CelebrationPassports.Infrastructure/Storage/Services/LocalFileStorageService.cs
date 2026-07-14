using CelebrationPassports.Infrastructure.Storage.Interfaces;
using Microsoft.Extensions.Hosting;

namespace CelebrationPassports.Infrastructure.Storage.Services;

// Placeholder implementation — saves to local disk under wwwroot/uploads. Replace with a
// real cloud blob storage provider (Azure Blob / S3 / DO Spaces) before deploy.
public class LocalFileStorageService : IFileStorageService
{
    private const string UploadsFolder = "uploads";

    private readonly string _uploadsPath;

    public LocalFileStorageService(IHostEnvironment environment)
    {
        _uploadsPath = Path.Combine(environment.ContentRootPath, "wwwroot", UploadsFolder);
    }

    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_uploadsPath);

        var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var filePath = Path.Combine(_uploadsPath, storedFileName);

        await using var fileStream = File.Create(filePath);
        await content.CopyToAsync(fileStream, cancellationToken);

        return $"/{UploadsFolder}/{storedFileName}";
    }
}
