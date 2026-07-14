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
        public async Task<UserSession?> GetActiveSessionByUserIdAsync(Guid userId, Guid sessionId)
        {
            return await _dbcontext.UserSessions

      .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == sessionId);
        }

        public async Task<UserSession?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbcontext.UserSessions
                .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        }
    }
}
