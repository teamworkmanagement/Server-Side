using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Statistics
{
    public class UserTaskDoneInBoardsRequest
    {
        public string UserId { get; set; }
        public string Filter { get; set; } //week/month/year
    }
}
