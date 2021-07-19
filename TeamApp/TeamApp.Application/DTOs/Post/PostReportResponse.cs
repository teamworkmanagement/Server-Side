using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Post
{
    public class PostReportResponse
    {
        public string Id { get; set; }
        public bool IsSelected { get; set; } = false;
        public bool IsShown { get; set; } = false;
        public string Content { get; set; }
        public string UserAvatar { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public string PostId { get; set; }
        public List<object> Images { get; set; }
    }
}
