using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.GroupChatUser
{
    public class GroupChatUserSeenRequest
    {
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public bool? IsSeen { get; set; }
    }
}
