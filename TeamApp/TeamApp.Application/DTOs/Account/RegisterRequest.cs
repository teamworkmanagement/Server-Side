using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Application.DTOs.Account
{
    public class RegisterRequest
    {
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        //[Required]
        //[MinLength(6)]
        //public string UserName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
