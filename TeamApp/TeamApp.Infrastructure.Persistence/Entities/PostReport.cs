using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public class PostReport
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string UserReportId { get; set; }
        public string Status { get; set; }
        public int ReportCount { get; set; } = 0;
    }
}
