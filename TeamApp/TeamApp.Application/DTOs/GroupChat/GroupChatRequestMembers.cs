using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.GroupChat
{
    public class GroupChatRequestMembers: GroupChatRequest
    {
        public List<string> Members { get; set; }
    }
}
