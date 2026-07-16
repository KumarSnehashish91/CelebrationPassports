namespace CelebrationPassports.Application.Imports.Interfaces;

public interface IImportProcessingService
{
    // Processes exactly one pending job end-to-end (unzip, upload each file, mark
    // Completed/Failed). Returns true if a job was found and processed, false if the
    // queue was empty — lets the background service loop tightly while work is queued
    // and back off when it isn't.
    Task<bool> ProcessNextPendingAsync(CancellationToken cancellationToken);
}
