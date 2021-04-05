using System;
using System.Collections.Generic;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class Comment
    {
        public string CommentId { get; set; }
        public string CommentPostId { get; set; }
        public string CommentUserId { get; set; }
        public string CommentContent { get; set; }
        public DateTime? CommentCreatedAt { get; set; }
        public bool? CommentIsDeleted { get; set; }
        public string CommentType { get; set; }

        public virtual Post CommentPost { get; set; }
        public virtual User CommentUser { get; set; }
    }
}
