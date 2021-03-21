using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.Team;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        public Task<string> AddTeam(TeamRequest teamReq)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTeam(string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<TeamResponse> GetById(string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TeamResponse>> GetByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateTeam(string teamId, TeamRequest teamReq)
        {
            throw new NotImplementedException();
        }
    }
}
