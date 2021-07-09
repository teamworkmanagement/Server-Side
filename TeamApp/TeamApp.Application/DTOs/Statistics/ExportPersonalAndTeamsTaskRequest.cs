using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Statistics
{
    public class ExportPersonalAndTeamsTaskRequest
    {
        public IFormFile Image { get; set; }
        public string UserStatis { get; set; }
        public string TeamStatis { get; set; }
    }
}
