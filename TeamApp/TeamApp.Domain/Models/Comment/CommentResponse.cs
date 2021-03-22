using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Domain.Models.Comment
{
    public class CommentResponse
    {
        public string CommentId { get; set; }
        public string CommentPostId { get; set; }
        public string CommentUserId { get; set; }
        public string CommentContent { get; set; }
        public DateTime? CommentCreatedAt { get; set; }
        public bool? CommentIsDeleted { get; set; }
    }
}
