using System;
using System.Collections.Generic;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
            PostReacts = new HashSet<PostReact>();
            Files = new HashSet<File>();
        }

        public string PostId { get; set; }
        public string PostUserId { get; set; }
        public string PostTeamId { get; set; }
        public string PostContent { get; set; }
        public DateTime? PostCreatedAt { get; set; }
        public int? PostCommentCount { get; set; }
        public bool? PostIsDeleted { get; set; }
        public bool? PostIsPinned { get; set; }

        public virtual Team PostTeam { get; set; }
        public virtual User PostUser { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<PostReact> PostReacts { get; set; }
        public virtual ICollection<File> Files { get; set; }
    }
}
