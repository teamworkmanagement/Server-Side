using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Application.DTOs.Account
{
    public class SocialRequest
    {
        public string Id { get; set; }
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string ImageUrl { get; set; }
    }
}
