using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.KanbanBoard
{
    public class SearchBoardModel
    {
        public string UserId { get; set; }
        public bool IsOfTeam { get; set; }
        public string TeamId { get; set; }
        public string KeyWord { get; set; }
    }
}
