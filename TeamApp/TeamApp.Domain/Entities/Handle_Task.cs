using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class Handle_Task
    {
        public string handle_task_id { get; set; }
        public string handle_task_user_id { get; set; }
        public string handle_task_task_id { get; set; }
        [Timestamp]
        public DateTimeOffset? handle_task_created_at { get; set; }
        public bool handle_task_is_deleted { get; set; }
    }
}
