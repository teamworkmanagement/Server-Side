using System;
using System.Collections.Generic;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class GroupChatUser
    {
        public string GroupChatUserId { get; set; }
        public string GroupChatUserUserId { get; set; }
        public string GroupChatUserGroupChatId { get; set; }
        public bool? GroupChatUserIsDeleted { get; set; }

        public virtual GroupChat GroupChatUserGroupChat { get; set; }
        public virtual User GroupChatUserUser { get; set; }
    }
}
