namespace CelebrationPassports.Persistence.Enums;

// gift-message-schedule-claim.md — gates claim-link visibility/claimability, not
// payment. Payment (mock, in this app) always completes at Finalize/Purchase time
// regardless of delivery mode.
public enum GiftDeliveryMode
{
    Immediate = 1,
    Scheduled = 2
}
