namespace CelebrationPassports.Persistence.Enums;

public enum ChapterSource
{
    Manual = 1,
    AiDetected = 2,

    // gift-message-schedule-claim.md — the first Chapter created in a recipient's
    // Passport on claim, pre-populated from the gift-giver's GeneratedStory/GiftPhotos.
    Gifted = 3
}
