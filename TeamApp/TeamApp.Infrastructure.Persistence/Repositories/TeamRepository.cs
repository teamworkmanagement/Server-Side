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

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IGroupChatRepository _groupChatRepository;
        private readonly IGroupChatUserRepository _groupChatUserRepository;
        private readonly IParticipationRepository _participationRepository;
        private readonly IKanbanBoardRepository _kanbanBoardRepository;
        private readonly IKanbanListRepository _kanbanListRepository;
        public TeamRepository(TeamAppContext dbContext, IGroupChatRepository groupChatRepository, IGroupChatUserRepository groupChatUserRepository
            , IParticipationRepository participationRepository, IKanbanBoardRepository kanbanBoardRepository, IKanbanListRepository kanbanListRepository)
        {
            _dbContext = dbContext;
            _groupChatRepository = groupChatRepository;
            _groupChatUserRepository = groupChatUserRepository;
            _participationRepository = participationRepository;
            _kanbanListRepository = kanbanListRepository;
            _kanbanBoardRepository = kanbanBoardRepository;
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

        public async Task<TeamResponse> GetById(string teamId)
        {
            var entity = await _dbContext.Team.FindAsync(teamId);
            if (entity == null)
                return null;

            return new TeamResponse
            {
                TeamId = entity.TeamId,
                TeamLeaderId = entity.TeamLeaderId,
                TeamName = entity.TeamName,
                TeamDescription = entity.TeamDescription,
                TeamCreatedAt = entity.TeamCreatedAt.FormatTime(),
                TeamCode = entity.TeamCode,
                TeamIsDeleted = entity.TeamIsDeleted,
            };
        }

        public async Task<List<TeamResponse>> GetByUserId(string userId)
        {
            var query = from p in _dbContext.Participation.AsNoTracking()
                        join t in _dbContext.Team.AsNoTracking() on p.ParticipationTeamId equals t.TeamId
                        join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                        select new { t, u, t.Participation };
            var teams = await query.AsNoTracking().Where(x => x.u.Id == userId).ToListAsync();
            var outPut = new List<TeamResponse>();

            foreach (var team in teams)
            {
                var leader = await _dbContext.User.FindAsync(team.t.TeamLeaderId);
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
                    TeamLeaderImageUrl = leader.ImageUrl,
                    TeamMemberCount = team.Participation.Count,
                    TeamImageUrl = string.IsNullOrEmpty(team.t.TeamImageUrl) ? $"https://ui-avatars.com/api/?name={team.t.TeamName}" : team.t.TeamImageUrl,
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

        public async Task<bool> UpdateTeam(string teamId, TeamRequest teamReq)
        {
            var entity = await _dbContext.Team.FindAsync(teamId);
            if (entity == null)
                return false;

            entity = new Team
            {
                TeamId = teamId,
                TeamLeaderId = teamReq.TeamLeaderId,
                TeamName = teamReq.TeamName,
                TeamDescription = teamReq.TeamDescription,
                TeamCreatedAt = teamReq.TeamCreatedAt,
                TeamCode = teamReq.TeamCode,
                TeamIsDeleted = teamReq.TeamIsDeleted
            };

            _dbContext.Team.Update(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResponse<UserResponse>> GetUsersByTeamIdPaging(TeamUserParameter userParameter)
        {
            var team = await _dbContext.Team.FindAsync(userParameter.TeamId);

            var query = from p in _dbContext.Participation.AsNoTracking()
                        join t in _dbContext.Team.AsNoTracking() on p.ParticipationTeamId equals t.TeamId
                        join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                        where t.TeamId == userParameter.TeamId && u.Id != team.TeamLeaderId
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
                               where t.TeamCode == request.TeamCode && p.ParticipationUserId == request.UserId
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


        public async Task<UserResponse> GetAdmin(string teamId)
        {
            var team = await _dbContext.Team.FindAsync(teamId);
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
                        where t.TeamId == teamId
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
    }
}
