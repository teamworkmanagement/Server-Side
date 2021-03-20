using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class Participation
    {
        public string participation_id { get; set; }
        public string participation_user_id { get; set; }
        public string participation_team_id { get; set; }
        [Timestamp]
        public DateTimeOffset? participation_created_at { get; set; }
        public bool participation_is_deleted { get; set; }
    }
}
