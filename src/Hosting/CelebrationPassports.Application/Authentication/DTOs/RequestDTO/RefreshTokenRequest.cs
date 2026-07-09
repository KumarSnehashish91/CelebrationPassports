using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Authentication.DTOs.RequestDTO
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
