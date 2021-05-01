using System;
using System.Collections.Generic;
using System.Text;
using TeamApp.Application.DTOs.Task;

namespace TeamApp.Application.DTOs.KanbanList
{
    public class KanbanListUIResponse
    {
        public string KanbanListId { get; set; }
        public string KanbanListTitle { get; set; }
        public string KanbanListBoardBelongedId { get; set; }
        public int? KanbanListOrderInBoard { get; set; }
        public List<TaskUIKanban> TaskUIKanbans { get; set; }
    }
}
