using System;
using System.Collections.Generic;
using System.Text;
using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities
{
    public class Notification 
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public NotificationType NotificationType { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public ReferenceType? ReferenceType { get; set; }

        public Guid? ReferenceId { get; set; }

        //public NotificationPriority Priority { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadOn { get; set; }

        public string? ActionUrl { get; set; }

        public DateTime? ExpiresOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public User User { get; set; } = null!;
    }
}
