using CelebrationPassports.Application.Users.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Users.Interfaces
{
    public interface IuserProfileService
    {
        Task<UserProfileDto?> GetUserProfileAsync(Guid userId);
    }
}
