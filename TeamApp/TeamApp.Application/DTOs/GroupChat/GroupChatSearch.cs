using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.GroupChat
{
    public class GroupChatSearch
    {
        public string UserId { get; set; }
        public string KeyWord { get; set; }
        public string CurrentGroup { get; set; }
        public bool IsSearch { get; set; } = false;
    }
}
