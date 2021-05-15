using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Notification
{
    public class NotificationResponse
    {
        public string NotificationId { get; set; }
        public string NotificationUserId { get; set; }
        public string NotificationContent { get; set; }
        public DateTime? NotificationCreatedAt { get; set; }
        public string NotificationLink { get; set; }
        public bool? NotificationStatus { get; set; } //true: unread, false: read
        public bool? NotificationIsDeleted { get; set; }
        public string NotificationImage { get; set; }
        public Dictionary<string,string> NotificationPayload { get; set; }
    }
}
