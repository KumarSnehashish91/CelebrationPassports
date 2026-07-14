using CelebrationPassports.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Persistence.Repositories.Interfaces
{
    public interface IUserSessionRepository
    {
        Task<UserSession?> GetActiveSessionByUserIdAsync(Guid userId, Guid sessionId);
        Task<UserSession?> GetByRefreshTokenAsync(string refreshToken);
        Task AddAsync(UserSession userSession);
    }
}
