using CelebrationPassports.Application.TimeCapsule.Configuration;
using CelebrationPassports.Application.TimeCapsule.Interfaces;
using Microsoft.Extensions.Options;

namespace CelebrationPassports.API.BackgroundServices;

// Time-Capsule Messages (feature-backlog-v1.1.md, RELIVE #6): "A scheduled job (daily
// is sufficient — no debounce needed here, unlike chapter clustering)". Plain periodic
// sweep, no cancel-and-reschedule logic — every message due gets unlocked whenever this
// next fires.
public class TimeCapsuleUnlockBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<TimeCapsuleOptions> _options;
    private readonly ILogger<TimeCapsuleUnlockBackgroundService> _logger;

    public TimeCapsuleUnlockBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<TimeCapsuleOptions> options,
        ILogger<TimeCapsuleUnlockBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunSweepAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Time-capsule unlock sweep failed.");
            }

            try
            {
                var interval = TimeSpan.FromMinutes(Math.Max(_options.CurrentValue.CheckIntervalMinutes, 1));
                await Task.Delay(interval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Shutting down.
            }
        }
    }

    private async Task RunSweepAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var unlockService = scope.ServiceProvider.GetRequiredService<ITimeCapsuleUnlockService>();

        var unlockedCount = await unlockService.UnlockDueMessagesAsync();

        if (unlockedCount > 0)
        {
            _logger.LogInformation("Time-capsule unlock sweep unlocked {Count} message(s).", unlockedCount);
        }
    }
}
