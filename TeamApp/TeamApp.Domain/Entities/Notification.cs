using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class Notification
    {
		public string notification_id { get; set; }
		public string notification_user_id { get; set; }
		public string notification_content { get; set; }
		public DateTimeOffset? notification_created_at { get; set; }
		public string notification_link { get; set; }
		public bool notification_status { get; set; }
		public bool notification_is_deleted { get; set; }
	}
}
