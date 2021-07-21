using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Comment
{
    public class CommentReportResponse
    {
        public string Id { get; set; }
        public bool IsSelected { get; set; } = false;
        public bool IsShown { get; set; } = false;
        public string Content { get; set; }
        public string UserAvatar { get; set; }
        public string UserName { get; set; }
        public string CommentId { get; set; }
        public int ReportCounts { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
