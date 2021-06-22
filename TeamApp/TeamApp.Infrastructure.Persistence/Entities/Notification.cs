using System;
using System.Collections.Generic;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class Notification
    {
        public string NotificationId { get; set; }
        public string NotificationUserId { get; set; }
        public string NotificationContent { get; set; }
        public DateTime? NotificationCreatedAt { get; set; }
        public string NotificationLink { get; set; }
        public bool? NotificationStatus { get; set; }
        public bool? NotificationIsDeleted { get; set; }
        public string NotificationGroup { get; set; }
        public string NotificationActionUserId { get; set; }

        public virtual User NotificationUser { get; set; }
        public virtual User NotificationActionUser { get; set; }
    }
}
