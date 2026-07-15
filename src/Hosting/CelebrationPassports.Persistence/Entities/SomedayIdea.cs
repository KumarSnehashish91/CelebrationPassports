namespace CelebrationPassports.Persistence.Entities;

// Feature: Someday List (feature-backlog-v1.1.md, PLAN #13). "Plan" in the BRD's
// terminology maps to this codebase's Event — same adaptation already established for
// "Trip" (see TripPlannerController) — so ConvertedToEventId is this feature's
// equivalent of the spec's ConvertedToPlanId.
public class SomedayIdea
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? ConvertedToEventId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Event? ConvertedToEvent { get; set; }

    public virtual User? DeletedByUser { get; set; }
}
