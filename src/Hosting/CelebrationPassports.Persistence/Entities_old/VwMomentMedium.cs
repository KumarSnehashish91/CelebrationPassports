using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class VwMomentMedium
{
    public Guid Id { get; set; }

    public Guid PassportMomentId { get; set; }

    public string MomentTitle { get; set; } = null!;

    public string MediaType { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public long? FileSize { get; set; }

    public string? MimeType { get; set; }

    public string? Caption { get; set; }

    public DateTime CreatedOn { get; set; }
}
