using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.GroupChat
{
    public class GroupChatRequest
    {
        public string GroupChatName { get; set; }
        public DateTime? GroupChatUpdatedAt { get; set; }
    }
}
