using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Post
{
    public class CreateReportRequest
    {
        public string UserReportId { get; set; }
        public string PostId { get; set; }
    }
}
