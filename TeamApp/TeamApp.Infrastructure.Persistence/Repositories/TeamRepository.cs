using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.DTOs.Team;
using TeamApp.Application.Utils;
using TeamApp.Application.DTOs.User;
using static TeamApp.Application.Utils.Extensions;
using TeamApp.Application.Wrappers;
using TeamApp.Application.DTOs.KanbanBoard;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using Microsoft.AspNetCore.SignalR;
using TeamApp.Infrastructure.Persistence.Hubs.App;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IGroupChatRepository _groupChatRepository;
        private readonly IParticipationRepository _participationRepository;
        private readonly IConfiguration _config;
        private readonly IHubContext<HubAppClient, IHubAppClient> _hubApp;
        public TeamRepository(TeamAppContext dbContext, IGroupChatRepository groupChatRepository, IGroupChatUserRepository groupChatUserRepository
            , IParticipationRepository participationRepository, IConfiguration config
            , IHubContext<HubAppClient, IHubAppClient> hubApp)
        {
            _dbContext = dbContext;
            _groupChatRepository = groupChatRepository;
            _participationRepository = participationRepository;
            _hubApp = hubApp;
            _config = config;
        }
        public async Task<TeamResponse> AddTeam(TeamRequest teamReq)
        {
            var teamCode = RadomString.RandomString(6);
            /*bool loop = false;
            while (!loop)
            {
                teamCode = RadomString.RandomString(6);
                var teamCheck = from t in _dbContext.Team.AsNoTracking()
                                where t.TeamCode == teamCode
                                select t;
                if (teamCheck == null)
                    loop = true;
            }*/

            var entity = new Team
            {
                TeamId = Guid.NewGuid().ToString(),
                TeamLeaderId = teamReq.TeamLeaderId,
                TeamName = teamReq.TeamName,
                TeamDescription = teamReq.TeamDescription,
                TeamCreatedAt = DateTime.UtcNow,
                TeamCode = teamCode,
                TeamIsDeleted = false,
            };
            await _dbContext.AddAsync(entity);
            var check = await _dbContext.SaveChangesAsync() > 0;

            if (check)
            {
                await _groupChatRepository.AddGroupChat(new Application.DTOs.GroupChat.GroupChatRequest
                {
                    GroupChatId = entity.TeamId,
                    GroupChatName = entity.TeamName,
                    GroupChatUpdatedAt = DateTime.UtcNow,
                    IsOfTeam = true,
                });

                await _participationRepository.AddParticipation(new Application.DTOs.Paricipation.ParticipationRequest
                {
                    ParticipationUserId = entity.TeamLeaderId,
                    ParticipationTeamId = entity.TeamId,
                });

                /*await _groupChatUserRepository.AddGroupChatUser(new Application.DTOs.GroupChatUser.GroupChatUserRequest
                {
                    GroupChatUserUserId = entity.TeamLeaderId,
                    GroupChatUserGroupChatId = entity.TeamId,
                    GroupChatUserIsDeleted = false,
                });


                await _kanbanBoardRepository.AddKanbanBoard(new Application.DTOs.KanbanBoard.KanbanBoardRequest
                {
                    KanbanBoardId = entity.TeamId,
                    KanbanBoardIsOfTeam = true,
                    KanbanBoardBelongedId = entity.TeamId,
                });

                await _kanbanListRepository.AddKanbanList(new Application.DTOs.KanbanList.KanbanListRequest
                {
                    KanbanListId = entity.TeamId,
                    KanbanListTitle = "Mới được thêm",
                    KanbanListBoardBelongedId = entity.TeamId,
                    KanbanListOrderInBoard = 0,
                });*/

                var leader = await _dbContext.User.FindAsync(teamReq.TeamLeaderId);

                return new TeamResponse
                {
                    TeamId = entity.TeamId,
                    TeamLeaderId = entity.TeamLeaderId,
                    TeamName = entity.TeamName,
                    TeamDescription = entity.TeamDescription,
                    TeamCreatedAt = entity.TeamCreatedAt,
                    TeamCode = entity.TeamCode,
                    TeamLeaderName = leader.FullName,
                    TeamLeaderImageUrl = leader.ImageUrl,
                    TeamMemberCount = 1,
                    TeamImageUrl = $"https://ui-avatars.com/api/?name={entity.TeamName}",
                };
            }
            return null;
        }

        public async Task<bool> DeleteTeam(string teamId)
        {
            var entity = await _dbContext.Team.FindAsync(teamId);
            if (entity == null)
                return false;

            entity.TeamIsDeleted = true;
            _dbContext.Team.Update(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<TeamResponse> GetById(string teamId, string userId)
        {
            var entity = await _dbContext.Team.FindAsync(teamId);
            if (entity == null)
                throw new KeyNotFoundException("Team not found");

            var par = await (from p in _dbContext.Participation.AsNoTracking()
                             where p.ParticipationUserId == userId && p.ParticipationTeamId == teamId
                             && p.ParticipationIsDeleted == false
                             select p).FirstOrDefaultAsync();

            if (par == null)
                throw new KeyNotFoundException("Team not found");

            return new TeamResponse
            {
                TeamId = entity.TeamId,
                TeamLeaderId = entity.TeamLeaderId,
                TeamName = entity.TeamName,
                TeamDescription = entity.TeamDescription,
                TeamCreatedAt = entity.TeamCreatedAt.FormatTime(),
                TeamCode = entity.TeamCode,
                TeamIsDeleted = entity.TeamIsDeleted,
                TeamImageUrl = string.IsNullOrEmpty(entity.TeamImageUrl) ? $"https://ui-avatars.com/api/?name={entity.TeamName}" : entity.TeamImageUrl,
            };
        }

        public async Task<List<TeamResponse>> GetByUserId(string userId)
        {
            var query = from p in _dbContext.Participation.AsNoTracking()
                        join t in _dbContext.Team.AsNoTracking() on p.ParticipationTeamId equals t.TeamId
                        join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                        where p.ParticipationIsDeleted == false
                        select new { t, u, t.Participation };

            var teams = await query.AsNoTracking().Where(x => x.u.Id == userId).ToListAsync();

            var outPut = new List<TeamResponse>();

            foreach (var team in teams)
            {
                var leader = await _dbContext.User.FindAsync(team.t.TeamLeaderId);
                var membersCount = await _dbContext.Participation.AsNoTracking().Where(
                    x => x.ParticipationTeamId == team.t.TeamId && x.ParticipationIsDeleted == false).CountAsync();

                List<TeamUserResponse> members = new List<TeamUserResponse>();
                if (membersCount != 0)
                {
                    var queryUsersTeam = from p in _dbContext.Participation.AsNoTracking()
                                         join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                                         where p.ParticipationTeamId == team.t.TeamId && p.ParticipationIsDeleted == false
                                         select new { u.FullName, u.ImageUrl };

                    members = await queryUsersTeam.Select(x => new TeamUserResponse
                    {
                        UserFullName = x.FullName,
                        UserImageUrl = string.IsNullOrEmpty(x.ImageUrl) ? $"https://ui-avatars.com/api/?name={x.FullName}" : x.ImageUrl,
                    }).Take(3).ToListAsync();
                }
                outPut.Add(new TeamResponse
                {
                    TeamId = team.t.TeamId,
                    TeamLeaderId = team.t.TeamLeaderId,
                    TeamName = team.t.TeamName,
                    TeamDescription = team.t.TeamDescription,
                    TeamCreatedAt = team.t.TeamCreatedAt.FormatTime(),
                    TeamCode = team.t.TeamCode,
                    TeamIsDeleted = team.t.TeamIsDeleted,
                    TeamLeaderName = leader.FullName,
                    TeamLeaderImageUrl = string.IsNullOrEmpty(leader.ImageUrl) ? $"https://ui-avatars.com/api/?name={leader.FullName}" : leader.ImageUrl,
                    TeamMemberCount = membersCount,
                    TeamImageUrl = string.IsNullOrEmpty(team.t.TeamImageUrl) ? $"https://ui-avatars.com/api/?name={team.t.TeamName}" : team.t.TeamImageUrl,
                    TeamUsers = members,
                });
            }
            /*var outPut = await query.Where(x => x.u.Id == userId).Select(entity => new TeamResponse
            {
                TeamId = entity.t.TeamId,
                TeamLeaderId = entity.t.TeamLeaderId,
                TeamName = entity.t.TeamName,
                TeamDescription = entity.t.TeamDescription,
                TeamCreatedAt = entity.t.TeamCreatedAt.FormatTime(),
                TeamCode = entity.t.TeamCode,
                TeamIsDeleted = entity.t.TeamIsDeleted,
            }).ToListAsync();*/

            return outPut;
        }

        public async Task<bool> UpdateTeam(TeamUpdateRequest teamUpdateRequest)
        {
            var entity = await _dbContext.Team.FindAsync(teamUpdateRequest.TeamId);
            if (entity == null)
                throw new KeyNotFoundException("Team not found");

            entity.TeamLeaderId = teamUpdateRequest.TeamLeaderId;
            entity.TeamName = teamUpdateRequest.TeamName;
            entity.TeamDescription = teamUpdateRequest.TeamDescription;
            entity.TeamImageUrl = teamUpdateRequest.TeamImageUrl;

            _dbContext.Team.Update(entity);
            await _dbContext.SaveChangesAsync();

            var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                 join uc in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals uc.UserId
                                 where p.ParticipationIsDeleted == false && p.ParticipationTeamId == teamUpdateRequest.TeamId
                                 select uc.ConnectionId).Distinct().ToListAsync();

            await _hubApp.Clients.Clients(clients).UpdateTeamInfo(new
            {
                TeamId = teamUpdateRequest.TeamId,
                Leader = false,
                Time = DateTime.UtcNow
            });

            return true;
        }

        public async Task<PagedResponse<UserResponse>> GetUsersByTeamIdPaging(TeamUserParameter userParameter)
        {
            var team = await _dbContext.Team.FindAsync(userParameter.TeamId);

            if (team == null)
                throw new KeyNotFoundException("Team not found");

            var query = from p in _dbContext.Participation.AsNoTracking()
                        join t in _dbContext.Team.AsNoTracking() on p.ParticipationTeamId equals t.TeamId
                        join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                        where t.TeamId == userParameter.TeamId && u.Id != team.TeamLeaderId && p.ParticipationIsDeleted == false
                        orderby p.ParticipationCreatedAt
                        select new { u, t };

            var count = await query.CountAsync();

            query = query.Skip((userParameter.PageNumber - 1) * userParameter.PageSize).Take(userParameter.PageSize);

            var outPut = await query.Select(ur => new UserResponse
            {
                UserId = ur.u.Id,
                UserEmail = ur.u.Email,
                UserFullname = ur.u.FullName,
                UserImageUrl = string.IsNullOrEmpty(ur.u.ImageUrl) ? $"https://ui-avatars.com/api/?name={ur.u.FullName}" : ur.u.ImageUrl,
                UserCreatedAt = ur.u.CreatedAt,
            }).ToListAsync();




            var pagedResponse = new PagedResponse<UserResponse>(outPut, userParameter.PageSize, count);

            return pagedResponse;
        }

        public async Task<TeamResponse> JoinTeam(JoinTeamRequest request)
        {
            var team = await (from t in _dbContext.Team.AsNoTracking()
                              where t.TeamCode == request.TeamCode
                              select t).FirstOrDefaultAsync();
            if (team == null)
                return null;


            var team2 = await (from t in _dbContext.Team.AsNoTracking()
                               join p in _dbContext.Participation.AsNoTracking() on t.TeamId equals p.ParticipationTeamId
                               where t.TeamCode == request.TeamCode && p.ParticipationUserId == request.UserId && p.ParticipationIsDeleted == false
                               select t).FirstOrDefaultAsync();
            //đã join
            if (team2 != null)
                return new TeamResponse
                {
                    TeamId = team.TeamId,
                };


            await _participationRepository.AddParticipation(new Application.DTOs.Paricipation.ParticipationRequest
            {
                ParticipationUserId = request.UserId,
                ParticipationTeamId = team.TeamId,
            });


            return new TeamResponse
            {
                TeamId = team.TeamId,
                TeamLeaderId = team.TeamLeaderId,
                TeamName = team.TeamName,
                TeamDescription = team.TeamDescription,
                TeamCreatedAt = team.TeamCreatedAt,
                TeamCode = team.TeamCode,
            };
        }


        public async Task<UserResponse> GetAdmin(string teamId, string userId)
        {
            var team = await _dbContext.Team.FindAsync(teamId);

            if (team == null)
                throw new KeyNotFoundException("Team not found");

            var par = await (from p in _dbContext.Participation.AsNoTracking()
                             where p.ParticipationUserId == userId && p.ParticipationTeamId == teamId
                             && p.ParticipationIsDeleted == false
                             select p).FirstOrDefaultAsync();

            if (par == null)
                throw new KeyNotFoundException("Team not found");

            var admin = await _dbContext.User.FindAsync(team.TeamLeaderId);
            if (admin == null)
                return null;

            var adminRes = new UserResponse
            {
                UserId = admin.Id,
                UserEmail = admin.Email,
                UserFullname = admin.FullName,
                UserImageUrl = string.IsNullOrEmpty(admin.ImageUrl) ? $"https://ui-avatars.com/api/?name={admin.FullName}" : admin.ImageUrl,
                UserCreatedAt = admin.CreatedAt,
            };
            return adminRes;
        }

        public async Task<List<UserResponse>> GetUsersForTag(string teamId)
        {
            var query = from p in _dbContext.Participation.AsNoTracking()
                        join t in _dbContext.Team.AsNoTracking() on p.ParticipationTeamId equals t.TeamId
                        join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                        where t.TeamId == teamId && p.ParticipationIsDeleted == false
                        select u;

            return await query.Select(u => new UserResponse
            {
                UserId = u.Id,
                UserFullname = u.FullName,
                UserImageUrl = string.IsNullOrEmpty(u.ImageUrl) ? $"https://ui-avatars.com/api/?name={u.FullName}" : u.ImageUrl,
            }).ToListAsync();
        }

        public async Task<List<KanbanBoardResponse>> GetBoardsByTeam(string teamId)
        {
            //get all team for user
            var team = await _dbContext.Team.FindAsync(teamId);
            if (team == null)
                throw new KeyNotFoundException("Team not found");

            List<KanbanBoardResponse> responses = new List<KanbanBoardResponse>();


            var boards = await (from b in _dbContext.KanbanBoard.AsNoTracking()
                                where b.KanbanBoardTeamId == teamId
                                select b).ToListAsync();

            foreach (var board in boards)
            {
                var taskCount = await (from t in _dbContext.Task.AsNoTracking()
                                       join kl in _dbContext.KanbanList.AsNoTracking() on t.TaskBelongedId equals kl.KanbanListId
                                       where kl.KanbanListBoardBelongedId == board.KanbanBoardId && t.TaskIsDeleted == false
                                       select t.TaskId).CountAsync();

                responses.Add(new KanbanBoardResponse
                {
                    KanbanBoardId = board.KanbanBoardId,
                    KanbanBoardUserId = board.KanbanBoardUserId,
                    KanbanBoardTeamId = board.KanbanBoardTeamId,
                    KanbanBoardName = board.KanbanBoardName,
                    TasksCount = taskCount,
                });
            }
            return responses;
        }

        public async Task<List<TeamRecommendModel>> GetRecommendTeamForUser(string userId)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");
            var user = await _dbContext.User.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var query = "select count(*) as GroupNewPostCount, teamOfUser.team_id as GroupId, teamOfUser.team_name as GroupName,team_image_url as GroupAvatar " +
                        "from(select distinct team.team_id, team.team_name, team.team_image_url " +
                        "from user " +
                        "join participation on user.user_id = participation.participation_user_id " +
                        "join team on team.team_id = participation.participation_team_id " +
                        $"where user.user_id = '{userId}' and participation.participation_is_deleted <> 1 ) teamOfUser " +

                        "join post " +
                        "on post.post_team_id = teamOfUser.team_id " +
                        "where date(post.post_created_at)= date(now()) " +
                        "group by post.post_team_id " +
                        "limit 5";
            var outPut = new List<TeamRecommendModel>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var counts = await connection.QueryAsync<TeamRecommendModel>(query);
                outPut = counts.ToList();
            }

            foreach (var ele in outPut)
            {
                var count = await _dbContext.Participation.AsNoTracking()
                    .Where(x => x.ParticipationTeamId == ele.GroupId).CountAsync();

                ele.GroupMemberCount = count;
            }

            if (outPut.Count != 5)
            {
                var append = 5 - outPut.Count;
                var listTeam = outPut.Select(x => x.GroupId).ToList();

                var queryTeam = (from p in _dbContext.Participation.AsNoTracking()
                                 join t in _dbContext.Team.AsNoTracking() on p.ParticipationTeamId equals t.TeamId
                                 where p.ParticipationUserId == userId && p.ParticipationIsDeleted == false && !listTeam.Contains(p.ParticipationTeamId)
                                 select new { t, t.Participation.Count }).Take(append);

                var appendList = await queryTeam.AsNoTracking().Select(x => new TeamRecommendModel
                {
                    GroupId = x.t.TeamId,
                    GroupName = x.t.TeamName,
                    GroupAvatar = x.t.TeamImageUrl,
                    GroupNewPostCount = 0,
                    GroupMemberCount = x.Count,
                }).ToListAsync();

                outPut.AddRange(appendList);
            }

            return outPut;
        }

        public async Task<bool> ChangeTeamLeader(ChangeTeamAdminModel changeTeamAdminModel)
        {
            var team = await _dbContext.Team.FindAsync(changeTeamAdminModel.TeamId);
            if (team == null)
                throw new KeyNotFoundException("Team not found");

            team.TeamLeaderId = changeTeamAdminModel.LeaderId;
            _dbContext.Update(team);

            await _dbContext.SaveChangesAsync();

            var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                 join uc in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals uc.UserId
                                 where p.ParticipationIsDeleted == false && p.ParticipationTeamId == changeTeamAdminModel.TeamId
                                 select uc.ConnectionId).Distinct().ToListAsync();

            await _hubApp.Clients.Clients(clients).UpdateTeamInfo(new
            {
                TeamId = changeTeamAdminModel.TeamId,
                LeaderId = changeTeamAdminModel.LeaderId,
                Time = DateTime.UtcNow
            });

            return true;
        }

        public async Task<PagedResponse<UserResponse>> GetUsersByTeamIdPagingSearch(TeamUserParameter userParameter)
        {
            var team = await _dbContext.Team.FindAsync(userParameter.TeamId);

            if (team == null)
                throw new KeyNotFoundException("Team not found");

            var query = from p in _dbContext.Participation.AsNoTracking()
                        join t in _dbContext.Team.AsNoTracking() on p.ParticipationTeamId equals t.TeamId
                        join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                        where t.TeamId == userParameter.TeamId && u.Id != team.TeamLeaderId && p.ParticipationIsDeleted == false
                        orderby p.ParticipationCreatedAt
                        select new { u, t };



            var listUsers = await query.ToListAsync();
            if (!string.IsNullOrEmpty(userParameter.KeyWord))
            {
                var keyWord = userParameter.KeyWord.UnsignUnicode();
                listUsers = listUsers.Where(x => x.u.FullName.UnsignUnicode().Contains(keyWord)).ToList();
            }

            var count = listUsers.Count();

            listUsers = listUsers.Skip((userParameter.PageNumber - 1) * userParameter.PageSize).Take(userParameter.PageSize).ToList();

            var outPut = listUsers.Select(ur => new UserResponse
            {
                UserId = ur.u.Id,
                UserEmail = ur.u.Email,
                UserFullname = ur.u.FullName,
                UserImageUrl = string.IsNullOrEmpty(ur.u.ImageUrl) ? $"https://ui-avatars.com/api/?name={ur.u.FullName}" : ur.u.ImageUrl,
                UserCreatedAt = ur.u.CreatedAt,
            }).ToList();

            var pagedResponse = new PagedResponse<UserResponse>(outPut, userParameter.PageSize, count);

            return pagedResponse;
        }
    }
}
