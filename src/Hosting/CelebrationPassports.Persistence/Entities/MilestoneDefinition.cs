using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class MilestoneDefinition
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public MilestoneMetricType MetricType { get; set; }

    public int TargetValue { get; set; }

    public virtual ICollection<PassportMilestoneProgress> PassportProgress { get; set; } = new List<PassportMilestoneProgress>();
}
