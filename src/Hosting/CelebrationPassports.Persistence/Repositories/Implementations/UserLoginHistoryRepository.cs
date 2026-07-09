using CelebrationPassports.Persistence.Context;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Persistence.Repositories.Implementations
{
    public class UserLoginHistoryRepository : GenericRepository<UserLoginHistory>, IUserLoginHistoryRepository
    {
        public UserLoginHistoryRepository(CelebrationPassportsDbContext context) : base(context)
        {
        }
        public async Task AddAsync(UserLoginHistory userLoginHistory)
        {
            await _dbcontext.UserLoginHistories.AddAsync(userLoginHistory);
        }
       

    }
}
