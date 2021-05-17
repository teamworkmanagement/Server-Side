using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Task
{
    public class TaskUIKanban
    {
        public int? OrderInList { get; set; }
        public string KanbanListId { get; set; }
        public string TaskId { get; set; }

        public string TaskImageUrl { get; set; }

        public string TaskName { get; set; }
        public DateTime? TaskStartDate { get; set; }
        public DateTime? TaskDeadline { get; set; }
        public string TaskDescription { get; set; }
        public string TaskStatus { get; set; }

        public int? CommentsCount { get; set; }
        public int FilesCount { get; set; } = 0;

        public string UserId { get; set; }
        public string UserAvatar { get; set; }

        public int? TaskCompletedPercent { get; set; }

        public string TaskThemeColor { get; set; }
    }
}
