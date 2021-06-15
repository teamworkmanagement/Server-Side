using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Paricipation
{
    public class ParticipationRequest
    {
        public bool IsByEmail { get; set; } = false;
        public string Email { get; set; }
        public string ParticipationUserId { get; set; }
        public string ParticipationTeamId { get; set; }
        public string ActionUserId { get; set; }
    }
}
