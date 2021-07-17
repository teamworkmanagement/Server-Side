using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Meeting
{
    public class InviteMemberModel
    {
        public string UserInvite { get; set; }
        public string MeetingId { get; set; }
        public List<string> UserIds { get; set; }
    }
}
