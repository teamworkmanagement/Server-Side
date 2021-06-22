using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.GroupChat
{
    public class CheckDoubleGroupChatExists
    {
        public string UserOneId { get; set; }
        public string  UserTwoId { get; set; }
    }
}
