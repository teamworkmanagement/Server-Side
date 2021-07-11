using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Statistics
{
    public class BoardPointAndDoneRequest
    {
        //public List<UsersTaskDoneAndPointResponse> RequestModels { get; set; }
        public string RequestModels { get; set; }
        public IFormFile Image { get; set; }
        public string BoardName { get; set; }
    }
}
