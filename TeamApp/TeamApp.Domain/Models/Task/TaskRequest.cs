using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Domain.Models.Task
{
    public class TaskRequest
    {
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public int? TaskPoint { get; set; }
        public DateTime? TaskCreatedAt { get; set; }
        public DateTime? TaskDeadline { get; set; }
        public string TaskStatus { get; set; }
        public int? TaskCompletedPercent { get; set; }
        public string TaskTeamId { get; set; }
        public bool? TaskIsDeleted { get; set; }
    }
}
