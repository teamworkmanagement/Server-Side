using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class File
    {
        public string file_id { get; set; }
        public string file_name { get; set; }
        public string file_url { get; set; }
        public string file_type { get; set; }
    }
}
