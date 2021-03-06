using System;
using System.Collections.Generic;
using System.Text;
using TeamApp.Application.DTOs.KanbanList;

namespace TeamApp.Application.DTOs.KanbanBoard
{
    public class KanbanBoardUIResponse
    {
        public string KanbanBoardId { get; set; }
        public bool? KanbanBoardIsOfTeam { get; set; }
        public string KanbanBoardUserId { get; set; }
        public string KanbanBoardTeamId { get; set; }
        public bool? AdminAction { get; set; } = false;
        public List<KanbanListUIResponse> KanbanListUIs { get; set; }
    }
}
