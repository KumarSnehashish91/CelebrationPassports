using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Exceptions
{

    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException(string email)
            : base($"An account with email '{email}' already exists.")
        {
        }
    }
}
