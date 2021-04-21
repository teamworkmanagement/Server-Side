using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public class PostReact
    {
        public string PostReactId { get; set; }
        public string PostReactPostId { get; set; }
        public string PostReactUserId { get; set; }

        public virtual User UserReact { get; set; }
        public virtual Post Post { get; set; }
    }
}
