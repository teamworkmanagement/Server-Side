using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Meeting
{
    public class MeetingResponse
    {
        public string MeetingId { get; set; }
        public string MeetingName { get; set; }
        public string UserCreateId { get; set; }
        public string UserCreateName { get; set; }
        public string UserCreateAvatar { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public string Status { get; set; }
        public string Password { get; set; }
    }
}
