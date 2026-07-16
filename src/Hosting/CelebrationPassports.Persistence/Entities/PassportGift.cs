using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

// Feature: Gift a Passport (feature-backlog-v1.1.md, PLAN #14). v1 uses a mock
// payment — Amount is recorded for display only, no real payment gateway is called.
// One row per gifted Passport, created at purchase time and updated (never re-created)
// when the recipient claims it via RedemptionCode.
public class PassportGift
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public Guid PurchasedByUserId { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string? RecipientEmail { get; set; }

    // Written text lives here (reused, not duplicated, per gift-message-schedule-claim.md
    // open question 1) when MessageType == Written; MessageMediaUrl holds the file for
    // Voice/Video instead. Null MessageType means no personal message was ever set.
    public string? GiftMessage { get; set; }

    public GiftMessageType? MessageType { get; set; }

    public string? MessageMediaUrl { get; set; }

    public GiftDeliveryMode DeliveryMode { get; set; } = GiftDeliveryMode.Immediate;

    // Null when DeliveryMode == Immediate. Gates claim-link visibility/claimability
    // only — payment (mock, in this app) already completed at Finalize/Purchase time.
    public DateTime? ScheduledDeliveryDate { get; set; }

    public decimal Amount { get; set; }

    // Unguessable, unique — the whole security model for claiming is "possession of
    // this code", same as a physical gift card. No signing/HMAC needed since claiming
    // always round-trips through the database anyway.
    public string RedemptionCode { get; set; } = string.Empty;

    public PassportGiftStatus Status { get; set; } = PassportGiftStatus.AwaitingClaim;

    public DateTime PurchasedOn { get; set; }

    public Guid? ClaimedByUserId { get; set; }

    public DateTime? ClaimedOn { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual User PurchasedByUser { get; set; } = null!;

    public virtual User? ClaimedByUser { get; set; }
}
