using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.TaskVersion
{
    public class TaskVersionRequest
    {
        public string TaskVersionTaskId { get; set; }
        public DateTime? TaskVersionUpdatedAt { get; set; }
        public string TaskVersionTaskName { get; set; }
        public string TaskVersionTaskDescription { get; set; }
        public int? TaskVersionTaskPoint { get; set; }
        public DateTime? TaskVersionTaskDeadline { get; set; }
        public DateTime? TaskVersionStartDate { get; set; }
        public DateTime? TaskVersionDoneDate { get; set; }
        public string TaskVersionTaskStatus { get; set; }
        public int? TaskVersionTaskCompletedPercent { get; set; }
        public string TaskVersionActionUserId { get; set; }
    }
}
