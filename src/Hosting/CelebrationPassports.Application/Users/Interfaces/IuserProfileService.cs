using CelebrationPassports.Application.Users.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Users.Interfaces
{
    public interface IuserProfileService
    {
        Task<UserProfileDto?> GetUserProfileAsync(Guid userId);

        // placeId null clears the home location (detection then simply stays inactive
        // for this user until they set one again).
        Task SetHomePlaceAsync(Guid userId, Guid? placeId);
    }
}
