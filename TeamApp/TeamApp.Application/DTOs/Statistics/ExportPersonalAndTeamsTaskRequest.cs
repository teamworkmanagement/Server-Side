using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Statistics
{
    public class ExportPersonalAndTeamsTaskRequest
    {
        public List<int> UserStatis { get; set; }
        public List<int> TeamStatis { get; set; }
    }
}
