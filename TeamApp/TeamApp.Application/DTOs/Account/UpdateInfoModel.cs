using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Account
{
    public class UpdateInfoModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserPhoneNumber { get; set; }
        public DateTime? UserDob { get; set; }
        public string UserAddress { get; set; }
        public string UserDescription { get; set; }
        public string UserGithubLink { get; set; }
        public string UserFacebookLink { get; set; }
    }
}
