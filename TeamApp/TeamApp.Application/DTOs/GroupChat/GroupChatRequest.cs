using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.GroupChat
{
    public class GroupChatRequest
    {
        public string GroupChatId { get; set; }
        public string GroupChatName { get; set; }
        public string GroupChatType { get; set; }
        public DateTime? GroupChatUpdatedAt { get; set; }
        public bool IsOfTeam { get; set; }
    }
}
