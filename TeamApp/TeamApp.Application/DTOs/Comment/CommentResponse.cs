using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Comment
{
    public class CommentResponse
    {
        public string CommentId { get; set; }
        public string CommentPostId { get; set; }
        public string CommentTaskId { get; set; }
        public string CommentUserId { get; set; }
        public string CommentContent { get; set; }
        public DateTime? CommentCreatedAt { get; set; }
        public bool? CommentIsDeleted { get; set; }
        public string UserAvatar { get; set; }
        public string UserName { get; set; }
    }
}
