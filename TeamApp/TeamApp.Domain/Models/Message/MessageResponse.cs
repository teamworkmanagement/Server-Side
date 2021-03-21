﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Domain.Models.Message
{
    public class MessageResponse
    {
        public string MessageId { get; set; }
        public string MessageUserId { get; set; }
        public string MessageGroupChatId { get; set; }
        public string MessageContent { get; set; }
        public DateTime? MessageCreatedAt { get; set; }
        public bool? MessageIsDeleted { get; set; }
    }
}