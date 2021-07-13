using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Task
{
    public class ReAssignModel
    {
        public string PrevUserId { get; set; }
        public string CurrentUserId { get; set; }
        public string TaskId { get; set; }
        public string UserActionId { get; set; }
        public string ReqConnectionId { get; set; }
    }
}
