using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class Task
    {
		public string task_id { get; set; }
		public string task_name { get; set; }
		public string task_description { get; set; }
		public int task_point { get; set; }
		[Timestamp]
		public DateTimeOffset? task_created_at { get; set; }
		[Timestamp]
		public DateTimeOffset? task_deadline { get; set; }
		public task_status task_status { get; set; }
		public int task_completed_percent { get; set; }
		public string task_team_id { get; set; }
		public bool task_is_deleted { get; set; }
	}
}
