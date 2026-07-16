namespace CelebrationPassports.Persistence.Enums;

// gift-message-schedule-claim.md — the gift-giver's personal message. Exclusive choice
// (the Step 5 UI is 3 cards, only one active), null on PassportGift/GiftDraft means no
// message was ever set.
public enum GiftMessageType
{
    Written = 1,
    Voice = 2,
    Video = 3
}
