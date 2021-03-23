using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Team;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface ITeamRepository
    {
        Task<List<TeamResponse>> GetByUserId(string userId);
        Task<TeamResponse> GetById(string teamId);
        Task<string> AddTeam(TeamRequest teamReq);
        Task<bool> UpdateTeam(string teamId, TeamRequest teamReq);
        Task<bool> DeleteTeam(string teamId);
    }
}
