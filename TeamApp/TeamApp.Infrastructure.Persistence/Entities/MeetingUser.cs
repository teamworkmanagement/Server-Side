using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public class MeetingUser
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserConnectionId { get; set; }
        public string MeetingId { get; set; }
    }
}
