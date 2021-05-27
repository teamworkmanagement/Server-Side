using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.KanbanBoard
{
    public class SwapListModel
    {
        public string KanbanBoardId { get; set; }
        public string Position { get; set; }
        public string KanbanListId { get; set; }
    }
}
