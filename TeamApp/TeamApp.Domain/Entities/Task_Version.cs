using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class Task_Version
    {
		public string task_version_id { get; set; }
		public string task_version_task_id { get; set; }
		[Timestamp]
		public DateTimeOffset? task_version_updated_at { get; set; }
		public string task_version_task_name { get; set; }
		public string task_version_task_description { get; set; }
		public int task_version_task_point { get; set; }
		public DateTimeOffset? task_version_task_deadline { get; set; }
		public task_status task_version_task_status { get; set; }
		public int task_version_task_completed_percent { get; set; }
		public bool task_version_task_is_deleted { get; set; }
	}
}
