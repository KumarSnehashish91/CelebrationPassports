using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class Configuration
{
    public Guid ConfigurationId { get; set; }

    public string Category { get; set; } = null!;

    public string ConfigurationKey { get; set; } = null!;

    public string ConfigurationValue { get; set; } = null!;

    public string DataType { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsEncrypted { get; set; }

    public bool IsEditable { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;
}
