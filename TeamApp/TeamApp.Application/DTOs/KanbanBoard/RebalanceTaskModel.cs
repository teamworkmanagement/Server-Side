using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.KanbanBoard
{
    public class RebalanceTaskModel
    {
        public string KanbanListId { get; set; }
        public List<string> Ranks { get; set; }
    }
}
