using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.HandleTask
{
    public class HandleTaskResponse
    {
        public string HandleTaskId { get; set; }
        public string HandleTaskUserId { get; set; }
        public string HandleTaskTaskId { get; set; }
        public DateTime? HandleTaskCreatedAt { get; set; }
        public bool? HandleTaskIsDeleted { get; set; }
    }
}
