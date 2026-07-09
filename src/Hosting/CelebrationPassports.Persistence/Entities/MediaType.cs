using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class MediaType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public short DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual ICollection<MomentMedium> MomentMedia { get; set; } = new List<MomentMedium>();
}
