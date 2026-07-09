using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Authentication.DTOs.ResponseDTO
{
    public sealed class LogoutResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LoggedOutOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public string? RevokedReason { get; set; }
    }
}
