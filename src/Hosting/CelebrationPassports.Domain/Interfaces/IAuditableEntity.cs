using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Domain.Interfaces
{
    public interface IAuditableEntity
    {
        DateTime CreatedOn { get; set; }

        DateTime? ModifiedOn { get; set; }
    }
}
