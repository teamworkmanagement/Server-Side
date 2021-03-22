using System;
using System.Collections.Generic;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class Participation
    {
        public string ParticipationId { get; set; }
        public string ParticipationUserId { get; set; }
        public string ParticipationTeamId { get; set; }
        public DateTime? ParticipationCreatedAt { get; set; }
        public bool? ParticipationIsDeleted { get; set; }

        public virtual Team ParticipationTeam { get; set; }
        public virtual User ParticipationUser { get; set; }
    }
}
