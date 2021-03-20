using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class Team
    {
		public string team_id { get; set; }
		public string team_leader_id { get; set; }
		public string team_name { get; set; }
		public string team_description { get; set; }
		[Timestamp]
		public DateTimeOffset? team_created_at { get; set; }
		public string team_code { get; set; }
		public bool team_is_deleted { get; set; }
	}
}
