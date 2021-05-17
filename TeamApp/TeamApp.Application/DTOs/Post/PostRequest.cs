using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Post
{
    public class PostRequest
    {
        public string PostUserId { get; set; }
        public string PostTeamId { get; set; }
        public string PostContent { get; set; }
        public DateTime? PostCreatedAt { get; set; }
        public int? PostCommentCount { get; set; }
        public bool? PostIsDeleted { get; set; }
        public bool? PostIsPinned { get; set; }
        public List<string> UserIds { get; set; }
        public List<PostImage> PostImages { get; set; }
    }
}
