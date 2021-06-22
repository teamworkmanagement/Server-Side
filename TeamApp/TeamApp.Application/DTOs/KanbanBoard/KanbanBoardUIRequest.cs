using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.KanbanBoard
{
    public class KanbanBoardUIRequest
    {
        public string BoardId { get; set; }
        public string OwnerId { get; set; }
        public bool IsOfTeam { get; set; }
    }
}
