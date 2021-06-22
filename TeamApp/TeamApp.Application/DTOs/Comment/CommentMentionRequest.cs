using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Comment
{
    public class CommentMentionRequest
    {
        public string ActionUserId { get; set; }
        public List<string> UserIds { get; set; }
        public string TaskId { get; set; }
        public string PostId { get; set; }
    }
}
