using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Comment
{
    public class CommentRequest
    {
        public string CommentPostId { get; set; }
        public string CommentTaskId { get; set; }
        public string CommentUserId { get; set; }
        public string CommentUserAvatar { get; set; }
        public string CommentUserName { get; set; }
        public string CommentTeamId { get; set; }
        public string CommentContent { get; set; }
        public DateTime? CommentCreatedAt { get; set; }
        public bool? CommentIsDeleted { get; set; }
        public List<string> CommentUserTagIds { get; set; }
    }
}
