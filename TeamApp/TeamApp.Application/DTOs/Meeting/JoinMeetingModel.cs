using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Meeting
{
    public class JoinMeetingModel
    {
        public string UserId { get; set; }
        public string UserConnectionId { get; set; }
        public string MeetingId { get; set; }
    }
}
