using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.GroupChat
{
    public class AddMembersRequest
    {
        public string GroupChatId { get; set; }
        public List<string> UserIds { get; set; }
    }
}
