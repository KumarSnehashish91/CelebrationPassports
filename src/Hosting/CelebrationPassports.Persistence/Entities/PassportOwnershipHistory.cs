namespace CelebrationPassports.Persistence.Entities;

public class PassportOwnershipHistory
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public Guid OldOwnerId { get; set; }

    public Guid NewOwnerId { get; set; }

    public DateTime TransferredOn { get; set; }

    public Guid TransferredBy { get; set; }

    public string? Reason { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual User OldOwner { get; set; } = null!;

    public virtual User NewOwner { get; set; } = null!;

    public virtual User TransferredByUser { get; set; } = null!;
}
