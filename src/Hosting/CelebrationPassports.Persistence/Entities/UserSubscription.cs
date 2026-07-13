using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class UserSubscription
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PlanId { get; set; }

    public SubscriptionStatus Status { get; set; }

    public DateOnly? RenewsAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual SubscriptionPlan Plan { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
