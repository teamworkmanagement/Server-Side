using System;
using System.Collections.Generic;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class Team
    {
        public Team()
        {
            Participation = new HashSet<Participation>();
            Post = new HashSet<Post>();
            Task = new HashSet<Task>();
        }

        public string TeamId { get; set; }
        public string TeamLeaderId { get; set; }
        public string TeamName { get; set; }
        public string TeamDescription { get; set; }
        public DateTime? TeamCreatedAt { get; set; }
        public string TeamCode { get; set; }
        public bool? TeamIsDeleted { get; set; }
        public string TeamImageUrl { get; set; }

        public virtual User TeamLeader { get; set; }
        public virtual ICollection<Participation> Participation { get; set; }
        public virtual ICollection<Post> Post { get; set; }
        public virtual ICollection<Task> Task { get; set; }
    }
}
