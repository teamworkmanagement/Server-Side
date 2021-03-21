using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class User
    {
        public User()
        {
            Comment = new HashSet<Comment>();
            GroupChatUser = new HashSet<GroupChatUser>();
            HandleTask = new HashSet<HandleTask>();
            Message = new HashSet<Message>();
            Notification = new HashSet<Notification>();
            Participation = new HashSet<Participation>();
            Post = new HashSet<Post>();
            Team = new HashSet<Team>();
        }

        
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserFullname { get; set; }
        public DateTime? UserDateOfBirth { get; set; }
        public string UsePhoneNumber { get; set; }
        public string UserImageUrl { get; set; }
        public DateTime? UserCreatedAt { get; set; }
        public bool? UserIsThemeLight { get; set; }

        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<GroupChatUser> GroupChatUser { get; set; }
        public virtual ICollection<HandleTask> HandleTask { get; set; }
        public virtual ICollection<Message> Message { get; set; }
        public virtual ICollection<Notification> Notification { get; set; }
        public virtual ICollection<Participation> Participation { get; set; }
        public virtual ICollection<Post> Post { get; set; }
        public virtual ICollection<Team> Team { get; set; }
    }
}
