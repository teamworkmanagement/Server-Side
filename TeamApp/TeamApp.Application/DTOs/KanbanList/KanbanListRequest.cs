using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.KanbanList
{
    public class KanbanListRequest
    {
        public string KanbanListId { get; set; }
        public string KanbanListTitle { get; set; }
        public string KanbanListBoardBelongedId { get; set; }
        public int? KanbanListOrderInBoard { get; set; }
    }
}
