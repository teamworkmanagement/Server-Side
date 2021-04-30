using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.KanbanBoard
{
    public class SwapListModel
    {
        public string KanbanBoardId { get; set; }
        public int SourceIndex { get; set; }
        public int DestinationIndex { get; set; }
    }
}
