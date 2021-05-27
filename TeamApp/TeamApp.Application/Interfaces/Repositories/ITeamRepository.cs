﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        Task<bool> UpdateTeam(string teamId, TeamRequest teamReq);
        Task<bool> DeleteTeam(string teamId);
        /// <summary>
        /// GetAll User of team
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        Task<PagedResponse<UserResponse>> GetUsersByTeamIdPaging(TeamUserParameter userParameter);
        Task<UserResponse> GetAdmin(string teamId);
        Task<TeamResponse> JoinTeam(JoinTeamRequest request);
    }
}
