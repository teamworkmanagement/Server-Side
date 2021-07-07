using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.AppSearch
{
    public class BoardSearchResponse
    {
        public string BoardId { get; set; }
        public string BoardName { get; set; }
        public string Link { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
