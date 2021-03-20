using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class Post
    {
		public string post_id { get; set; }
		public string post_user_id { get; set; }
		public string post_team_id { get; set; }
		public string post_content { get; set; }
		[Timestamp]
		public DateTimeOffset? post_created_at { get; set; }
		public int post_comment_count { get; set; }
		public bool post_is_deleted { get; set; }
		public bool post_is_pinned { get; set; }
	}
}
