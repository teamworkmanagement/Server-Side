using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Application.DTOs.Account
{
    public class ChangePasswordModel
    {
        [Required]
        public string UserId { get; set; }

        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassWord { get; set; }
    }
}
