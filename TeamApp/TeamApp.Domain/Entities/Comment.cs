using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class Comment
    {
		public string comment_id { get; set; }
		public string comment_post_id { get; set; }
		public string comment_user_id { get; set; }
		public string comment_content { get; set; }
		[Timestamp]
		public DateTimeOffset? comment_created_at { get; set; }
		public bool comment_is_deleted { get; set; }
	}
}
