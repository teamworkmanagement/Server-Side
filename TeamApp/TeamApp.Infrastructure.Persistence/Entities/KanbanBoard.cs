using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public class KanbanBoard
    {
        public KanbanBoard()
        {
            KanbanLists = new HashSet<KanbanList>();
        }
        public string KanbanBoardId { get; set; }
        public bool? KanbanBoardIsOfTeam { get; set; }
        public string KanbanBoardBelongedId { get; set; }
        public string KanbanBoardName { get; set; }
        public DateTime? KanbanBoardCreatedAt { get; set; }

        public virtual ICollection<KanbanList> KanbanLists { get; set; }
    }
}
