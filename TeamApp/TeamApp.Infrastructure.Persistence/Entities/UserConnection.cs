using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public class UserConnection
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
        public string Type { get; set; }
    }
}
