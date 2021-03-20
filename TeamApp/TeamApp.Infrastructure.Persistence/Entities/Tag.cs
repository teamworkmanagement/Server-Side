using System;
using System.Collections.Generic;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class Tag
    {
        public string TagId { get; set; }
        public string TagContent { get; set; }
        public string TagLink { get; set; }
    }
}
