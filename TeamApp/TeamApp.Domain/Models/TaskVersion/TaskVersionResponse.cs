using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Domain.Models.TaskVersion
{
    public class TaskVersionResponse
    {
        public string TaskVersionId { get; set; }
        public string TaskVersionTaskId { get; set; }
        public DateTime? TaskVersionUpdatedAt { get; set; }
        public string TaskVersionTaskName { get; set; }
        public string TaskVersionTaskDescription { get; set; }
        public int? TaskVersionTaskPoint { get; set; }
        public DateTime? TaskVersionTaskDeadline { get; set; }
        public string TaskVersionTaskStatus { get; set; }
        public int? TaskVersionTaskCompletedPercent { get; set; }
        public bool? TaskVersionTaskIsDeleted { get; set; }
    }
}
