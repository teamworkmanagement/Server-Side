using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Task
{
    public class TaskUpdateRequest
    {
        public string UserActionId { get; set; }
        public string TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public int? TaskPoint { get; set; }
        public DateTime? TaskCreatedAt { get; set; }
        public DateTime? TaskStartDate { get; set; }
        public DateTime? TaskDeadline { get; set; }
        public string TaskStatus { get; set; }
        public int? TaskCompletedPercent { get; set; }
        public string TaskTeamId { get; set; }
        public bool? TaskIsDeleted { get; set; }
        public string TaskBelongedId { get; set; }
        public int? TaskOrderInList { get; set; }
        public string TaskThemeColor { get; set; }
        public string TaskImageUrl { get; set; }
    }
}
