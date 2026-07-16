using CelebrationPassports.Application.Imports.Interfaces;

namespace CelebrationPassports.API.BackgroundServices;

// Feature: Import Existing Memories (feature-backlog-v1.1.md, PRESERVE #1) — this app has
// no job scheduler (Hangfire/Quartz/Redis), so unzip/upload work happens here instead of
// inline on the upload request, same simple-polling fallback already used by
// AutoChapterClusteringBackgroundService. Unlike that sweep, this drains the queue
// tightly (processes jobs back-to-back with no delay) since a user is actively watching
// the Import status page waiting for their job to finish, and only backs off once the
// queue is empty.
public class ImportJobProcessingBackgroundService : BackgroundService
{
    private static readonly TimeSpan IdlePollInterval = TimeSpan.FromSeconds(5);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ImportJobProcessingBackgroundService> _logger;

    public ImportJobProcessingBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ImportJobProcessingBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var processedAny = false;

            try
            {
                processedAny = await RunOnceAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Import job processing pass failed.");
            }

            if (processedAny)
            {
                continue;
            }

            try
            {
                await Task.Delay(IdlePollInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Shutting down.
            }
        }
    }

    private async Task<bool> RunOnceAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var processingService = scope.ServiceProvider.GetRequiredService<IImportProcessingService>();

        return await processingService.ProcessNextPendingAsync(ct);
    }
}
