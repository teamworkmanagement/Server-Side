using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.KanbanBoard
{
    public class RebalanceListModel
    {
        public string KanbanBoardId { get; set; }
        public List<string> Ranks { get; set; }
    }
}
