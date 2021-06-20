using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanBoard;
using TeamApp.Application.DTOs.Team;
using TeamApp.Application.DTOs.User;
using TeamApp.Application.Wrappers;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface ITeamRepository
    {
        Task<List<TeamResponse>> GetByUserId(string userId);
        Task<TeamResponse> GetById(string teamId);
        Task<TeamResponse> AddTeam(TeamRequest teamReq);
        Task<bool> UpdateTeam(TeamUpdateRequest teamUpdateRequest);
        Task<bool> DeleteTeam(string teamId);
        /// <summary>
        /// GetAll User of team
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        Task<PagedResponse<UserResponse>> GetUsersByTeamIdPaging(TeamUserParameter userParameter);
        Task<UserResponse> GetAdmin(string teamId);
        Task<TeamResponse> JoinTeam(JoinTeamRequest request);
        Task<List<UserResponse>> GetUsersForTag(string teamId);
        Task<List<KanbanBoardResponse>> GetBoardsByTeam(string teamId);
        Task<List<TeamRecommendModel>> GetRecommendTeamForUser(string userId);
        Task<bool> ChangeTeamLeader(ChangeTeamAdminModel changeTeamAdminModel);
    }
}
