using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Domain.Models.Paricipation;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IParticipationRepository
    {
        Task<List<ParticipationResponse>> GetAllByTeamId(string teamId);
        Task<List<string>> GetTeamByUserId(string userId);
        Task<bool> DeleteParticipation(string userId, string teamId);
    }
}
