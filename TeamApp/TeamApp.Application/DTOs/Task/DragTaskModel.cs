using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Task
{
    public class DragTaskModel
    {
        public string TaskId { get; set; }
        public string Position { get; set; }
        public string OldList { get; set; }
        public string NewList { get; set; }
    }
}
