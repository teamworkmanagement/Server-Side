using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Meeting
{
    public class MeetingRequest
    {
        public string UserCreateId { get; set; }
        public string MeetingName { get; set; }
        public string TeamId { get; set; }
        public string Status { get; set; } = "meeting";
        public string ConnectionId { get; set; }
    }
}
