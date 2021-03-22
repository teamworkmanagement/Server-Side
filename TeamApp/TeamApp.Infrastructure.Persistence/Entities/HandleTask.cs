using System;
using System.Collections.Generic;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class HandleTask
    {
        public string HandleTaskId { get; set; }
        public string HandleTaskUserId { get; set; }
        public string HandleTaskTaskId { get; set; }
        public DateTime? HandleTaskCreatedAt { get; set; }
        public bool? HandleTaskIsDeleted { get; set; }

        public virtual Task HandleTaskTask { get; set; }
        public virtual User HandleTaskUser { get; set; }
    }
}
