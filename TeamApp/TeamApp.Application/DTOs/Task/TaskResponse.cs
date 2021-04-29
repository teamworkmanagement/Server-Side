using System;
using System.Collections.Generic;
using System.Text;
using TeamApp.Application.DTOs.Comment;
using TeamApp.Application.DTOs.File;

namespace TeamApp.Application.DTOs.Task
{
    public class TaskResponse
    {
        public string KanbanListId { get; set; }
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
        public string TaskThemeColor { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public int? OrderInList { get; set; }
        public List<CommentResponse> Comments { get; set; }
        public List<FileResponse> Files { get; set; }
    }
}
