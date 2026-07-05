using CelebrationPassports.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Domain.Common
{
    public abstract class BaseEntity : IAuditableEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
