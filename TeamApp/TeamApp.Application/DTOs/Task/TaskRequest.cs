using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Task
{
    public class TaskRequest
    {
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public int? TaskPoint { get; set; }
        public DateTime? TaskCreatedAt { get; set; }
        public DateTime? TaskStartDate { get; set; }
        public string TaskStatus { get; set; }
        public int? TaskCompletedPercent { get; set; }
        public string TaskTeamId { get; set; }
        public bool? TaskIsDeleted { get; set; }
        public string TaskBelongedId { get; set; }
        public string TaskRankInList { get; set; }
        public string TaskThemeColor { get; set; }
        public string TaskImageUrl { get; set; }
        public DateTime? TaskDeadline { get; set; }
        public string UserActionId { get; set; }
    }
}
