namespace CelebrationPassports.Persistence.Enums;

public enum GiftDraftStatus
{
    DraftPhotos = 1,
    DraftInsights = 2,
    DraftStory = 3,
    DraftPrint = 4,

    // Step 5 (gift-message-schedule-claim.md) — personal message + delivery schedule.
    DraftMessage = 5,

    Sent = 6
}
