namespace CelebrationPassports.Persistence.Enums;

// Tracks whether a GiftPhoto's insight (and, by extension, a GeneratedStory paragraph
// built from it) came from the gift-giver or was AI-written — drives the "Your words /
// AI-written" legend in the Story Preview screen.
public enum ParagraphOrigin
{
    User = 1,
    Ai = 2
}
