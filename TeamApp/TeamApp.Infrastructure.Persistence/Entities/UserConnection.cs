using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public class UserConnection
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
        public virtual User User { get; set; }
    }
}
