using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.GroupChatUser
{
    public class GroupChatUserResponse
    {
        public string GroupChatUserId { get; set; }
        public string GroupChatUserUserId { get; set; }
        public string GroupChatUserGroupChatId { get; set; }
        public bool? GroupChatUserIsDeleted { get; set; }
    }
}
