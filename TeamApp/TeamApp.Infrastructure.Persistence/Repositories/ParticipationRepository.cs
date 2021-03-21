using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.Paricipation;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class ParticipationRepository : IParticipationRepository
    {
        public Task<bool> DeleteParticipation(string userId, string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ParticipationResponse>> GetAllByTeamId(string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetTeamByUserId(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
