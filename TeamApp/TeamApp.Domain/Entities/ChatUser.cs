using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class ChatUser
    {
        public string group_chat_user_id { get; set; }
        public string group_chat_user_user_id { get; set; }
        public string group_chat_user_group_chat_id { get; set; }
        public bool group_chat_user_is_deleted { get; set; }
    }
}
