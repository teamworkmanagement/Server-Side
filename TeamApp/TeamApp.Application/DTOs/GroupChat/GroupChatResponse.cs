using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.GroupChat
{
    public class GroupChatResponse
    {
        public string GroupChatId { get; set; }
        public string GroupChatName { get; set; }
        public DateTime? GroupChatUpdatedAt { get; set; }
    }
}
