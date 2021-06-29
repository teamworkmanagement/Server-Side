using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Feedback
{
    public class FeedbackRequest
    {
        public string UserFeedbackId { get; set; }
        public string FeedbackContent { get; set; }
    }
}
