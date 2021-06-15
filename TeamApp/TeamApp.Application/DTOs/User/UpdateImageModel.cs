using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.User
{
    public class UpdateImageModel
    {
        public string UserId { get; set; }
        public string ImageUrl { get; set; }
        public bool Delete { get; set; }
    }
}
