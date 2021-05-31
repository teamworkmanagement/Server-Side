using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.KanbanBoard
{
    public class KanbanBoardResponse
    {
        public string KanbanBoardId { get; set; }
        public bool? KanbanBoardIsOfTeam { get; set; }
        public string KanbanBoardTeamId { get; set; }
        public string KanbanBoardUserId { get; set; }
        public string KanbanBoardName { get; set; }
        public string KanbanBoardGroupName { get; set; }
        public int? TasksCount { get; set; }
        public string GroupImageUrl { get; set; }
    }
}
