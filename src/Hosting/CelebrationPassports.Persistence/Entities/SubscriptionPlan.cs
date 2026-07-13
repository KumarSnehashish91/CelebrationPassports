using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class SubscriptionPlan
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public BillingCycle BillingCycle { get; set; }

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
