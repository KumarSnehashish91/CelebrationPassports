using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class Payment
{
    public Guid Id { get; set; }

    public Guid SubscriptionId { get; set; }

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual UserSubscription Subscription { get; set; } = null!;
}
