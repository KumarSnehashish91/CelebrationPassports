using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class Status
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public short DisplayOrder { get; set; }

    public bool IsSystem { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<PassportMoment> PassportMoments { get; set; } = new List<PassportMoment>();

    public virtual ICollection<Passport> Passports { get; set; } = new List<Passport>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
