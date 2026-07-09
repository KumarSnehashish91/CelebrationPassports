using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class VwPassportMoment
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string PassportTitle { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Location { get; set; }

    public DateTime MomentDate { get; set; }

    public int StatusId { get; set; }

    public int? TotalMedia { get; set; }

    public DateTime CreatedOn { get; set; }
}
