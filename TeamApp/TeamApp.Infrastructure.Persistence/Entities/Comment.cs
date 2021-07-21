using System;
using System.Collections.Generic;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class Comment
    {
        public Comment()
        {
            CommentReports = new HashSet<CommentReport>();
        }
        public string CommentId { get; set; }
        public string CommentPostId { get; set; }
        public string CommentUserId { get; set; }
        public string CommentTaskId { get; set; }
        public string CommentContent { get; set; }
        public DateTime? CommentCreatedAt { get; set; }
        public bool? CommentIsDeleted { get; set; }
        public string CommentType { get; set; }

        public virtual Post CommentPost { get; set; }
        public virtual User CommentUser { get; set; }
        public virtual Task CommentTask { get; set; }
        public virtual ICollection<CommentReport> CommentReports { get; set; }
    }
}
