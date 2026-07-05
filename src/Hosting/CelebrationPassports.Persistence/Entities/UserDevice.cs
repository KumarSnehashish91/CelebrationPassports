using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Entities;

[Index("UserId", Name = "IX_UserDevices_UserId")]
public partial class UserDevice
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [StringLength(200)]
    public string? DeviceName { get; set; }

    [StringLength(50)]
    public string? DeviceType { get; set; }

    [StringLength(200)]
    public string? DeviceIdentifier { get; set; }

    public DateTime? LastLoginOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserDevices")]
    public virtual User User { get; set; } = null!;
}
