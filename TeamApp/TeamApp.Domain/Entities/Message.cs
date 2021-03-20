using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class Message
    {
		public string message_id { get; set; }
		public string message_user_id { get; set; }
		public string message_group_chat_id { get; set; }
		public string message_content { get; set; }
		[Timestamp]
		public DateTimeOffset? message_created_at { get; set; }
		public bool message_is_deleted { get; set; }
	}
}
