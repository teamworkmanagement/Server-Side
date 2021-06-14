using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Statistics
{
    public class UsersTaskDoneAndPointResponse
    {
        public string UserFullName { get; set; }
        public int TaskDoneCount { get; set; }
        public int Point { get; set; }
        public string ColorCode { get; set; }
    }
}
