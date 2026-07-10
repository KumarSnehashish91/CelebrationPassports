using CelebrationPassports.Persistence.Context;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Persistence.Repositories.Implementations
{
    public class UserProfileRepository : GenericRepository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(CelebrationPassportsDbContext context) : base(context)
        {
        }

        async Task<UserProfile?> IUserProfileRepository.GetUserProfileByIdAsync(Guid userId)
        {
            return await _dbcontext.UserProfiles
      .AsNoTracking()
      .FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
