using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Users.DTOs
{
    public class UserProfileDto
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? DisplayName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? MobileNumber { get; set; }

        public string? ProfilePhotoUrl { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? HomePlaceId { get; set; }
    }
}
