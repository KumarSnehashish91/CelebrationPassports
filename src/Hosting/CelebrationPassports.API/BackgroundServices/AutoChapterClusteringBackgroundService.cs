using CelebrationPassports.Application.Stories.Configuration;
using CelebrationPassports.Application.Stories.Interfaces;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MediaEntity = CelebrationPassports.Persistence.Entities.Media;

namespace CelebrationPassports.API.BackgroundServices;

// Debounce mechanism for auto-chapter-detection.md (src/Data), Section 5 "Fallback"
// option — this project has no job scheduler (Hangfire/Quartz/Redis), so this is the
// simpler periodic sweep the spec explicitly allows in that case: every PollInterval,
// pick up any unassigned, clustering-pending media whose upload has "settled" for at
// least DebounceMinutes, and run clustering once per uploader. Less precise than a true
// cancel-and-reschedule debounce, but requires no new infrastructure.
public class AutoChapterClusteringBackgroundService : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(1);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AutoChapterClusteringBackgroundService> _logger;

    public AutoChapterClusteringBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<AutoChapterClusteringBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunSweepAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Auto-chapter clustering sweep failed.");
            }

            try
            {
                await Task.Delay(PollInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Shutting down.
            }
        }
    }

    private async Task RunSweepAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediaRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<MediaEntity>>();
        var clusteringService = scope.ServiceProvider.GetRequiredService<IAutoChapterClusteringService>();
        var options = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<ChapterClusteringOptions>>().CurrentValue;

        var cutoff = DateTime.UtcNow.AddMinutes(-options.DebounceMinutes);

        var stalePending = await mediaRepository.FindAsync(m =>
            m.ChapterId == null && m.PendingClustering && !m.IsDeleted && m.UploadedOn <= cutoff);

        var userIds = stalePending.Select(m => m.UploadedBy).Distinct().ToList();

        foreach (var userId in userIds)
        {
            ct.ThrowIfCancellationRequested();

            var result = await clusteringService.ClusterUnassignedMediaForUserAsync(userId);

            if (result.ChaptersCreated > 0)
            {
                _logger.LogInformation(
                    "Auto-chapter clustering for user {UserId}: {ChaptersCreated} chapter(s) created, {MediaAssigned} media assigned, {MediaStillUnassigned} still unassigned.",
                    userId, result.ChaptersCreated, result.MediaAssigned, result.MediaStillUnassigned);
            }
        }
    }
}
