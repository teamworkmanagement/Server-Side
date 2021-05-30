using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.User
{
    public class UserSearchModel
    {
        public string UserId { get; set; }
        public string Keyword { get; set; }
        public bool IsEmail { get; set; } = false;
    }
}
