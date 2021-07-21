using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public class CommentReport
    {
        public string CommentReportId { get; set; }
        public string CommentReportCommentId { get; set; }
        public string CommentReportUserId { get; set; }

        public virtual User UserReport { get; set; }
        public virtual Comment Comment { get; set; }
    }
}
