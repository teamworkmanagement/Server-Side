using System;
using System.Collections.Generic;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class Message
    {
        public string MessageId { get; set; }
        public string MessageUserId { get; set; }
        public string MessageGroupChatId { get; set; }
        public string MessageContent { get; set; }
        public DateTime? MessageCreatedAt { get; set; }
        public bool? MessageIsDeleted { get; set; }
        public bool? IsMessage { get; set; }
        public string MessageType { get; set; }

        public virtual GroupChat MessageGroupChat { get; set; }
        public virtual User MessageUser { get; set; }
    }
}
