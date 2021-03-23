﻿using Microsoft.AspNetCore.Identity;
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
        }


        public string FullName { get; set; }
        public DateTime? Dob { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsThemeLight { get; set; }



        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<GroupChatUser> GroupChatUser { get; set; }
        public virtual ICollection<HandleTask> HandleTask { get; set; }
        public virtual ICollection<Message> Message { get; set; }
        public virtual ICollection<Notification> Notification { get; set; }
        public virtual ICollection<Participation> Participation { get; set; }
        public virtual ICollection<Post> Post { get; set; }
        public virtual ICollection<Team> Team { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }


        public bool OwnsToken(string token)
        {
            return this.RefreshTokens?.Find(x => x.Token == token) != null;
        }
    }
}
