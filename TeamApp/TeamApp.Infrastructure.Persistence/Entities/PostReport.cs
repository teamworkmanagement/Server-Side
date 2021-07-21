using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public class PostReport
    {
        public string PostReportId { get; set; }
        public string PostReportPostId { get; set; }
        public string PostReportUserId { get; set; }

        public virtual User UserReport { get; set; }
        public virtual Post Post { get; set; }
    }
}
