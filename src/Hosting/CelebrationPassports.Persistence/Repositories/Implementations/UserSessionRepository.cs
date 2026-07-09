using CelebrationPassports.Persistence.Context;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Persistence.Repositories.Implementations
{
    public sealed class UserSessionRepository : GenericRepository<UserSession>, IUserSessionRepository
    {
        public UserSessionRepository(CelebrationPassportsDbContext context) : base(context)
        {
        }
        public async Task<UserSession?> GetActiveSessionByUserIdAsync(Guid userId)
        {
            return await _dbcontext.UserSessions
      .AsNoTracking()
      .FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
