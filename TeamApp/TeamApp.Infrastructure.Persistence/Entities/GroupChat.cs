using System;
using System.Collections.Generic;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class GroupChat
    {
        public GroupChat()
        {
            GroupChatUser = new HashSet<GroupChatUser>();
            Message = new HashSet<Message>();
        }

        public string GroupChatId { get; set; }
        public string GroupChatName { get; set; }
        public DateTime? GroupChatUpdatedAt { get; set; }

        public virtual ICollection<GroupChatUser> GroupChatUser { get; set; }
        public virtual ICollection<Message> Message { get; set; }
    }
}
