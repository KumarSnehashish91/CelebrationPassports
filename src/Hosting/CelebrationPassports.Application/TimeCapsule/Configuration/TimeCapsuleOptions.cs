namespace CelebrationPassports.Application.TimeCapsule.Configuration;

public class TimeCapsuleOptions
{
    // The spec calls "daily is sufficient — no debounce needed here". Checking hourly
    // by default is still well within that spirit (no debounce logic, just a plain
    // periodic sweep) while keeping unlocks reasonably prompt; configurable in case
    // that trade-off needs to move either way.
    public int CheckIntervalMinutes { get; set; } = 60;
}
