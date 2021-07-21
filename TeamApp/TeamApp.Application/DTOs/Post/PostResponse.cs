using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Post
{
    public class PostResponse
    {
        public string PostId { get; set; }
        public string PostUserId { get; set; }
        public string PostTeamId { get; set; }
        public string PostContent { get; set; }
        public DateTime? PostCreatedAt { get; set; }
        public int? PostCommentCount { get; set; }
        public int? PostReactCount { get; set; }
        public bool? PostIsDeleted { get; set; }
        public string UserAvatar { get; set; } = null;
        public string UserName { get; set; }
        public string TeamName { get; set; }
        public bool IsReacted { get; set; }
        public bool? ShowDelete { get; set; } = false;
        public List<string> PostImages { get; set; }
    }
}
