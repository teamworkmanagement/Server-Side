using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.KanbanBoard
{
    public class TaskSearchModel
    {
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string TaskStatus { get; set; }
        public string BoardId { get; set; }
        public DateTime? StartRange { get; set; }
        public DateTime? EndRange { get; set; }
        public string UserAssignId { get; set; }
    }
}
