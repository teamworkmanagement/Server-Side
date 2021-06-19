using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.KanbanBoard
{
    public class TaskSearchModel
    {
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string BoardId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
