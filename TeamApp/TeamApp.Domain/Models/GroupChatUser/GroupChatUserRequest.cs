﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Domain.Models.GroupChatUser
{
    public class GroupChatUserRequest
    {
        public string GroupChatUserUserId { get; set; }
        public string GroupChatUserGroupChatId { get; set; }
        public bool? GroupChatUserIsDeleted { get; set; }
    }
}
