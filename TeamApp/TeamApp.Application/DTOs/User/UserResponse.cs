using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.User
{
    public class UserResponse
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserFullname { get; set; }
        public DateTime? UserDateOfBirth { get; set; }
        public string UsePhoneNumber { get; set; }
        public string UserImageUrl { get; set; }
        public DateTime? UserCreatedAt { get; set; }
        public bool? UserIsThemeLight { get; set; }
    }
}
