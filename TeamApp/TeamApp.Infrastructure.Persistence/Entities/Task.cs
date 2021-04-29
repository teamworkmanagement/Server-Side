using System;
using System.Collections.Generic;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class Task
    {
        public Task()
        {
            HandleTask = new HashSet<HandleTask>();
            TaskVersion = new HashSet<TaskVersion>();
            Comments = new HashSet<Comment>();
        }

        public string TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public int? TaskPoint { get; set; }
        public DateTime? TaskCreatedAt { get; set; }
        public DateTime? TaskDeadline { get; set; }
        public string TaskStatus { get; set; }
        public int? TaskCompletedPercent { get; set; }
        public string TaskTeamId { get; set; }
        public bool? TaskIsDeleted { get; set; }
        public string TaskBelongedId { get; set; }
        public int? TaskOrderInList { get; set; }
        public string TaskThemeColor { get; set; }
        public double? TaskProgress { get; set; }

        public virtual Team TaskTeam { get; set; }
        public virtual ICollection<HandleTask> HandleTask { get; set; }
        public virtual ICollection<TaskVersion> TaskVersion { get; set; }
        public virtual KanbanList KanbanList { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
