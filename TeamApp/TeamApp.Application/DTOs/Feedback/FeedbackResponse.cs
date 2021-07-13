using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Feedback
{
    public class FeedbackResponse
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Status { get; set; }
        public bool? IsSelected { get; set; }
        public bool? IsShown { get; set; }
    }
}
