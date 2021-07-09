using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Statistics
{
    public class BoardDoneTaskExportRequest
    {
        public IFormFile Image { get; set; }
        public string BoardTaskDone { get; set; }
    }
}
