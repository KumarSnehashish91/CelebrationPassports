using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Features.Users.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public int StatusId { get; set; }

        public DateTime? EmailVerifiedOn { get; set; }
    }
}
