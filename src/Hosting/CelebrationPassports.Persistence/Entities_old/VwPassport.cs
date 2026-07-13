using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class VwPassport
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int StatusId { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? TotalMembers { get; set; }

    public int? TotalMoments { get; set; }
}
