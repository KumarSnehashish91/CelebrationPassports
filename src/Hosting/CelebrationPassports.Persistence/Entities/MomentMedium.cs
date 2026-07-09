using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class MomentMedium
{
    public Guid Id { get; set; }

    public Guid PassportMomentId { get; set; }

    public int MediaTypeId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public long? FileSize { get; set; }

    public string? MimeType { get; set; }

    public string? Caption { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual MediaType MediaType { get; set; } = null!;

    public virtual PassportMoment PassportMoment { get; set; } = null!;

    public virtual ICollection<PassportMoment> PassportMoments { get; set; } = new List<PassportMoment>();

    public virtual ICollection<Passport> Passports { get; set; } = new List<Passport>();
}
