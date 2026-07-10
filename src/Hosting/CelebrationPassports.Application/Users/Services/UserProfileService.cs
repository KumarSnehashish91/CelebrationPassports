using CelebrationPassports.Application.Users.DTOs;
using CelebrationPassports.Application.Users.Interfaces;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Users.Services
{
    public class UserProfileService : IuserProfileService
    {
        private readonly IUserProfileRepository _IuserProfileRepository;
        public UserProfileService(IUserProfileRepository userProfileRepository)
        {
            _IuserProfileRepository = userProfileRepository;
        }
        async Task<UserProfileDto?> IuserProfileService.GetUserProfileAsync(Guid userId)
        {
            var profile = await _IuserProfileRepository.GetUserProfileByIdAsync(userId);
            if (profile == null)
                return null;
            return new UserProfileDto
            {
                UserId = profile.UserId,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                DisplayName = profile.DisplayName,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                MobileNumber = profile.MobileNumber,
                ProfilePhotoUrl = profile.ProfilePhotoUrl,
                CreatedOn = profile.CreatedOn
            };
            
        }

    }
}
