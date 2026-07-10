using CelebrationPassports.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Persistence.Repositories.Interfaces
{
    public interface IUserProfileRepository : IGenericRepository<UserProfile>
    {
        Task<UserProfile?> GetUserProfileByIdAsync(Guid userId);
    }
}
