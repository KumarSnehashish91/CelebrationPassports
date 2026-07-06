using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Features.Authentication.DTOs
{
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
