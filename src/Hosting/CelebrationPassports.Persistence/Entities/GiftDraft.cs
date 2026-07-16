using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

// Feature: Gift Photo Story & Print-Ready Preview (gift-story-print-preview.md) —
// extends Gift a Passport (feature-backlog-v1.1.md, PLAN #14). A GiftDraft is the
// gift-giver's in-progress authoring session (photos → insights → story → print
// format); it has no Passport of its own until Finalize creates one and links
// PassportGiftId, at which point the underlying purchase is the same PassportGift
// flow already built for the simple (no-story) gift path.
public class GiftDraft
{
    public Guid Id { get; set; }

    public Guid SenderUserId { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string? RecipientEmail { get; set; }

    public string? PassportTitle { get; set; }

    public string? PersonalMessage { get; set; }

    public GiftDraftStatus Status { get; set; } = GiftDraftStatus.DraftPhotos;

    public PrintFormat? PrintFormat { get; set; }

    // Step 5 (gift-message-schedule-claim.md) — staged here during the wizard, copied
    // onto PassportGift at Finalize. PersonalMessage above IS the "Written" message
    // text when MessageType == Written; MessageMediaUrl holds the file otherwise.
    public GiftMessageType? MessageType { get; set; }

    public string? MessageMediaUrl { get; set; }

    public GiftDeliveryMode DeliveryMode { get; set; } = GiftDeliveryMode.Immediate;

    public DateTime? ScheduledDeliveryDate { get; set; }

    // Set only once Finalize succeeds.
    public Guid? PassportGiftId { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual User SenderUser { get; set; } = null!;

    public virtual PassportGift? PassportGift { get; set; }

    public virtual ICollection<GiftPhoto> Photos { get; set; } = new List<GiftPhoto>();

    public virtual GeneratedStory? Story { get; set; }
}
