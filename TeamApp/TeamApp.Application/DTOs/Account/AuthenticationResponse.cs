using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TeamApp.Application.DTOs.Account
{
    public class AuthenticationResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        //public List<string> Roles { get; set; }
        public bool IsVerified { get; set; }
        public string JWToken { get; set; }
        public string RefreshToken { get; set; }
        public string FullName { get; set; }
        public string UserAvatar { get; set; }
        public DateTime? UserDob { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserAddress { get; set; }
        public string UserDescription { get; set; }
        public string UserGithubLink { get; set; }
        public string UserFacebookLink { get; set; }
        public bool FirstTimeSocial { get; set; } = false;
    }
}
