namespace CelebrationPassports.Infrastructure.Storage.Interfaces;

public interface IFileStorageService
{
    // Returns the relative URL the file is servable at (e.g. "/uploads/{name}").
    Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken = default);

    // Reads back the bytes behind a URL previously returned by SaveAsync. First real
    // consumer: Gift Story photo insight generation, which needs to hand raw image
    // bytes to a vision-capable model — every earlier consumer only ever served the
    // URL to a browser via static files and never needed to re-read it server-side.
    Task<byte[]> ReadAsync(string url, CancellationToken cancellationToken = default);
}
