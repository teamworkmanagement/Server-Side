using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.GroupChat
{
    public class CustomListGroupChatResponse
    {
        public List<GroupChatResponse> GroupChats { get; set; }
        public string CurrentGroup { get; set; }
    }
}
