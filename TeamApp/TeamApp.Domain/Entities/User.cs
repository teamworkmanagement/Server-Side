using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class User
    {
        public string user_id { get; set; }
        public string user_email { get; set; }
        public string user_password { get; set; }
        public string user_fullname { get; set; }
        [Timestamp]
        public DateTimeOffset? user_date_of_birth { get; set; }
        public string use_phone_number { get; set; }
        public string user_image_url { get; set; }
        [Timestamp]
        public DateTimeOffset? user_created_at { get; set; }
        public bool user_is_theme_light { get; set; }
    }
}
