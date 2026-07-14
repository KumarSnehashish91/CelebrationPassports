namespace CelebrationPassports.Persistence.Entities;

public class PassportMilestoneProgress
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public Guid MilestoneId { get; set; }

    public int CurrentValue { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? CompletedOn { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual MilestoneDefinition Milestone { get; set; } = null!;
}
