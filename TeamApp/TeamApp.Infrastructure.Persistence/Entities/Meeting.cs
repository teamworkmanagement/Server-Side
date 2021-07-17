using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public class Meeting
    {
        public string MeetingId { get; set; }
        public string MeetingName { get; set; }
        public string UserCreateId { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public string TeamId { get; set; }
        public string Status { get; set; } //pending, end, meeting
        public string Password { get; set; }
    }
}
