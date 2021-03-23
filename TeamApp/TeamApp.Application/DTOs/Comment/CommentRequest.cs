using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Comment
{
    public class CommentRequest
    {
        public string CommentPostId { get; set; }
        public string CommentUserId { get; set; }
        public string CommentContent { get; set; }
        public DateTime? CommentCreatedAt { get; set; }
        public bool? CommentIsDeleted { get; set; }
    }
}
