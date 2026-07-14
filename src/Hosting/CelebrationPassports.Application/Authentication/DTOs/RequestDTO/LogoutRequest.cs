using CelebrationPassports.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Authentication.DTOs.RequestDTO
{
    public sealed class LogoutRequest
    {
        public Guid SessionId { get; set; }
    }
}
