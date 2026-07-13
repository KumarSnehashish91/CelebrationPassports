using CelebrationPassports.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Persistence.Repositories.Interfaces
{
    public interface IUserLoginHistoryRepository 
    {
        Task AddAsync(UserLoginHistory userLoginHistory);
        Task<UserLoginHistory?> GetLoginUserHistoryAsync(Guid userId);

    }
}
