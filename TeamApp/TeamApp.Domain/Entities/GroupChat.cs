using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class GroupChat
    {
        public string group_chat_id { get; set; }
        public string group_chat_name { get; set; }
        [Timestamp]
        public DateTimeOffset? group_chat_updated_at { get; set; }
    }
}
