using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Features.Authentication.DTOs
{
    public class AuthenticationResponse
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime ExpiresOn { get; set; }
    }
}
