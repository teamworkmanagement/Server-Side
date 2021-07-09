using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Statistics
{
    public class TaskModalResponse
    {
        public string TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskStatus { get; set; }
        public DateTime? TaskDeadline { get; set; }
        public string TaskDescription { get; set; }
        public string TaskImage { get; set; }
        public string Link { get; set; }
    }
}
