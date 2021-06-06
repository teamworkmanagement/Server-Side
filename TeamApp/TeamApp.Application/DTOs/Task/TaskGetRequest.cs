using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Task
{
    public class TaskGetRequest
    {
        public string BoardId { get; set; }
        public string TaskId { get; set; }
        public bool IsOfTeam { get; set; }
        public string OwnerId { get; set; }
    }
}
