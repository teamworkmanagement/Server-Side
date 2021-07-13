using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public class Feedback
    {
        public string FeedbackId { get; set; }
        public string FeedbackContent { get; set; }
        public string UserFeedbackId { get; set; }
        public DateTime? FeedbackCreatedAt { get; set; }
        public bool? IsSeen { get; set; } = false;

        public virtual User UserFeedback { get; set; }
    }
}
