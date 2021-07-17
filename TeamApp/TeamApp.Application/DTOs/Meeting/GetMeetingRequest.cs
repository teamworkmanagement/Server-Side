using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Meeting
{
    public class GetMeetingRequest
    {
        public string UserId { get; set; }
        public string MeetingId { get; set; }
        public string TeamId { get; set; }
    }
}
