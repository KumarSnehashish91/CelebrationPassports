namespace CelebrationPassports.Application.TimeCapsule.Interfaces;

public interface ITimeCapsuleUnlockService
{
    // Flips IsUnlocked on every due message and notifies each Passport member with an
    // account. Returns how many messages unlocked, for the background service to log.
    Task<int> UnlockDueMessagesAsync();
}
