using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.User
{
    public class UserExistsChatAddModel: UserSearchModel
    {
        public string GroupChatId { get; set; }
    }
}
