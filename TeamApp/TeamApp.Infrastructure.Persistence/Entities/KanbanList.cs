using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public class KanbanList
    {
        public KanbanList()
        {
            Tasks = new HashSet<Task>();
        }
        public string KanbanListId { get; set; }
        public string KanbanListTitle{ get; set; }
        public string KanbanListBoardBelongedId { get; set; }
        public string KanbanListRankInBoard { get; set; }
        public bool? KanbanListIsDeleted { get; set; }
        public bool? KanbanListDefault { get; set; }

        public virtual KanbanBoard KanbanBoard { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
