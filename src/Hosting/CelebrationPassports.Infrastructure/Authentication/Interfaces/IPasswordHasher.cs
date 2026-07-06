using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Infrastructure.Authentication.Interfaces
{
  
    public interface IPasswordHasher
    {
        string HashPassword(string password);

        bool VerifyPassword(string password, string passwordHash);
    }
}
