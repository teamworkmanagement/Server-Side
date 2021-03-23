using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Paricipation
{
    public class ParticipationResponse
    {
        public string ParticipationId { get; set; }
        public string ParticipationUserId { get; set; }
        public string ParticipationTeamId { get; set; }
        public DateTime? ParticipationCreatedAt { get; set; }
        public bool? ParticipationIsDeleted { get; set; }
    }
}
