using CelebrationPassports.Application.Exceptions;
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
        private readonly IUnitOfWork _unitOfWork;

        public UserProfileService(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
        {
            _IuserProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
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
                CreatedOn = profile.CreatedOn,
                HomePlaceId = profile.HomePlaceId
            };

        }

        public async Task SetHomePlaceAsync(Guid userId, Guid? placeId)
        {
            var profile = await _IuserProfileRepository.GetUserProfileByIdAsync(userId)
                ?? throw new NotFoundException("User profile not found.");

            // GetUserProfileByIdAsync reads with AsNoTracking, so the mutation below is
            // on a detached entity — Update() re-attaches it as Modified so
            // SaveChangesAsync actually persists it, instead of silently doing nothing.
            profile.HomePlaceId = placeId;
            _IuserProfileRepository.Update(profile);

            await _unitOfWork.SaveChangesAsync();
        }

    }
}
