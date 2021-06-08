using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TeamApp.Application.DTOs.Account;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class User : IdentityUser
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
            RefreshTokens = new List<RefreshToken>();
            FilesUpload = new HashSet<File>();
            FilesOwner = new HashSet<File>();
            PostReacts = new HashSet<PostReact>();
            Boards = new HashSet<KanbanBoard>();
            NotificationActionUsers = new HashSet<Notification>();
        }


        public string FullName { get; set; }
        public DateTime? Dob { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastTimeOnline { get; set; }
        public bool? FirstTimeSocial { get; set; }
        public string UserDescription { get; set; }
        public string UserAddress { get; set; }
        public string UserGithubLink { get; set; }
        public string UserFacebookLink { get; set; }



        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<GroupChatUser> GroupChatUser { get; set; }
        public virtual ICollection<HandleTask> HandleTask { get; set; }
        public virtual ICollection<Message> Message { get; set; }
        public virtual ICollection<Notification> Notification { get; set; }
        public virtual ICollection<Notification> NotificationActionUsers { get; set; }
        public virtual ICollection<Participation> Participation { get; set; }
        public virtual ICollection<Post> Post { get; set; }
        public virtual ICollection<Team> Team { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
        public List<UserConnection> UserConnections { get; set; }
        public virtual ICollection<File> FilesUpload { get; set; }
        public virtual ICollection<File> FilesOwner { get; set; }
        public virtual ICollection<PostReact> PostReacts { get; set; }
        public virtual ICollection<KanbanBoard> Boards { get; set; }


        public bool OwnsToken(string token)
        {
            return this.RefreshTokens?.Find(x => x.Token == token) != null;
        }
    }
}
