namespace CelebrationPassports.Persistence.Enums;

public enum PassportStatus
{
    Draft = 1,
    Active = 2,
    Archived = 3,

    // Gift a Passport (feature-backlog-v1.1.md, PLAN #14) — a purchased-but-unclaimed
    // gift. OwnerUserId holds the purchaser until PassportGift.ClaimAsync transfers it
    // to the recipient, at which point this flips to Active.
    GiftPending = 4
}
