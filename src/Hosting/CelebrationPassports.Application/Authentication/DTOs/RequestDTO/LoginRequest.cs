using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Authentication.DTOs.RequestDTO
{
    public sealed class LoginRequest
    {
        public string EmailAddress { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
