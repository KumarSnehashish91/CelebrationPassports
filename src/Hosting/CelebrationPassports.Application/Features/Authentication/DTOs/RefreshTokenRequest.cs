using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Features.Authentication.DTOs
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
