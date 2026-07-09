using CelebrationPassports.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Persistence.Repositories.Interfaces
{
    public interface IUserSessionRepository
    {
        Task<UserSession?> GetActiveSessionByUserIdAsync(Guid userId);
        Task AddAsync(UserSession userSession);
    }
}
