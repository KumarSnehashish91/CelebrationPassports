using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class SessionLocationInfo
{
    public Guid SessionLocationInfoId { get; set; }

    public Guid VisitorSessionId { get; set; }

    public string? Ipaddress { get; set; }

    public string? Country { get; set; }

    public string? CountryCode { get; set; }

    public string? State { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? TimeZone { get; set; }

    public string? Isp { get; set; }

    public string? Organization { get; set; }

    public string? Asn { get; set; }

    public string? ConnectionType { get; set; }

    public bool IsVpn { get; set; }

    public bool IsProxy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual VisitorSession VisitorSession { get; set; } = null!;
}
