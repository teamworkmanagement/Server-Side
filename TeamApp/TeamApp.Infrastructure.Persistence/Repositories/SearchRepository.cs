using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.AppSearch;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using TeamApp.Application.Wrappers;
using TeamApp.Application.Utils;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        private readonly TeamAppContext _dbContext;
        public SearchRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<PagedResponse<BoardSearchResponse>> SearchBoards(AppSearchRequest appSearchRequest)
        {
            //user boards
            var userBoards = from kb in _dbContext.KanbanBoard
                             where kb.KanbanBoardUserId == appSearchRequest.UserId
                             orderby kb.KanbanBoardCreatedAt descending
                             select new { kb.KanbanBoardId, kb.KanbanBoardName, kb.KanbanBoardCreatedAt };

            var userBoardsResoponse = await userBoards.Select(b => new BoardSearchResponse
            {
                BoardId = b.KanbanBoardId,
                BoardName = b.KanbanBoardName,
                BoardUserId = appSearchRequest.UserId,
                Link = $"/managetask/mytasks?&b={b.KanbanBoardId}",
                CreatedDate = b.KanbanBoardCreatedAt,
            }).ToListAsync();

            //user teams
            var userTeams = await (from t in _dbContext.Team
                                   join p in _dbContext.Participation on t.TeamId equals p.ParticipationTeamId
                                   where p.ParticipationIsDeleted == false && p.ParticipationUserId == appSearchRequest.UserId
                                   select t.TeamId).Distinct().ToListAsync();

            //all boards of teams
            var allBoardsTeams = from kb in _dbContext.KanbanBoard
                                 where userTeams.Contains(kb.KanbanBoardTeamId)
                                 orderby kb.KanbanBoardCreatedAt descending
                                 select new { kb.KanbanBoardId, kb.KanbanBoardName, kb.KanbanBoardTeamId, kb.KanbanBoardCreatedAt };

            var allBoardsTeamsResponse = await allBoardsTeams.Select(b => new BoardSearchResponse
            {
                BoardId = b.KanbanBoardId,
                BoardName = b.KanbanBoardName,
                Link = $"/managetask/teamtasks?gr={b.KanbanBoardTeamId}&b={b.KanbanBoardId}",
                CreatedDate = b.KanbanBoardCreatedAt,
                BoardTeamId = b.KanbanBoardTeamId,
            }).ToListAsync();


            foreach (var boardRes in allBoardsTeamsResponse)
            {
                var team = await _dbContext.Team.FindAsync(boardRes.BoardTeamId);
                boardRes.BoardTeamName = team.TeamName;
                boardRes.BoardTeamImage = string.IsNullOrEmpty(team.TeamImageUrl) ? $"https://ui-avatars.com/api/?name={team.TeamName}" : team.TeamImageUrl;
            }

            var response = new List<BoardSearchResponse>();
            response.AddRange(userBoardsResoponse);
            response.AddRange(allBoardsTeamsResponse);


            response = response.Where(r => r.BoardName.UnsignUnicode().Contains(appSearchRequest.KeyWord.UnsignUnicode())).ToList();
            var count = response.Count();

            response = response.OrderByDescending(b => b.CreatedDate)
                               .Skip(appSearchRequest.SkipItems)
                               .Take(appSearchRequest.PageSize)
                               .ToList();

            return new PagedResponse<BoardSearchResponse>(response, appSearchRequest.PageSize, count, skipRows: appSearchRequest.SkipItems);
        }

        public async Task<PagedResponse<GroupChatSearchResponse>> SearchGroupChats(AppSearchRequest appSearchRequest)
        {
            var results = await (from grc in _dbContext.GroupChat.AsNoTracking()
                                 join grcu in _dbContext.GroupChatUser.AsNoTracking() on grc.GroupChatId equals grcu.GroupChatUserGroupChatId
                                 where grcu.GroupChatUserIsDeleted == false && grcu.GroupChatUserUserId == appSearchRequest.UserId
                                 orderby grc.GroupChatUpdatedAt descending
                                 select new
                                 {
                                     grc.GroupChatId,
                                     grc.GroupChatName,
                                     grc.GroupChatIsOfTeam,
                                     grc.GroupChatImageUrl,
                                 }).Distinct().ToListAsync();

            results = results.Where(r => r.GroupChatName != null && r.GroupChatName.UnsignUnicode().Contains(appSearchRequest.KeyWord.UnsignUnicode())).ToList();
            var count = results.Count();

            var response = results.Select(r => new GroupChatSearchResponse
            {
                ChatId = r.GroupChatId,
                ChatName = r.GroupChatName,
                Link = $"chat?g={r.GroupChatId}",
                ChatIsOfTeam = r.GroupChatIsOfTeam,
                ChatImage = r.GroupChatImageUrl,
            }).Skip(appSearchRequest.SkipItems).Take(appSearchRequest.PageSize).ToList();

            //https://ui-avatars.com/api/?name={user.FullName}
            foreach (var res in response)
            {
                if (string.IsNullOrEmpty(res.ChatImage))
                {
                    if (res.ChatIsOfTeam == true)
                    {
                        var team = await _dbContext.Team.FindAsync(res.ChatId);
                        if (string.IsNullOrEmpty(team.TeamImageUrl))
                        {
                            res.ChatImage = $"https://ui-avatars.com/api/?name={team.TeamName}";
                        }
                    }
                    else
                    {
                        res.ChatImage = $"https://ui-avatars.com/api/?name={res.ChatName}";
                    }
                }

                var members = await (from gru in _dbContext.GroupChatUser.AsNoTracking()
                                     join u in _dbContext.User.AsNoTracking() on gru.GroupChatUserUserId equals u.Id
                                     where gru.GroupChatUserIsDeleted == false && gru.GroupChatUserGroupChatId == res.ChatId
                                     select new { Id = u.Id, MemberName = u.FullName, MemberAvatar = u.ImageUrl })
                                     .Distinct().Take(3).ToListAsync();

                res.ChatListMembers = members.Select(m => new MemberChat
                {
                    Id = m.Id,
                    MemberName = m.MemberName,
                    MemberAvatar = string.IsNullOrEmpty(m.MemberAvatar) ? $"https://ui-avatars.com/api/?name={m.MemberName}" : m.MemberAvatar,
                }).ToList();
            }

            return new PagedResponse<GroupChatSearchResponse>(response, appSearchRequest.PageSize, count, skipRows: appSearchRequest.SkipItems);
        }

        public async Task<PagedResponse<TaskSearchResponse>> SearchTasks(AppSearchRequest appSearchRequest)
        {
            //user boards
            var userBoards = await (from kb in _dbContext.KanbanBoard
                                    where kb.KanbanBoardUserId == appSearchRequest.UserId
                                    orderby kb.KanbanBoardCreatedAt descending
                                    select kb.KanbanBoardId).ToListAsync();

            //user tasks
            var userTasks = from b in _dbContext.KanbanBoard.AsNoTracking()
                            join kl in _dbContext.KanbanList.AsNoTracking() on b.KanbanBoardId equals kl.KanbanListBoardBelongedId
                            join t in _dbContext.Task.AsNoTracking() on kl.KanbanListId equals t.TaskBelongedId
                            where kl.KanbanListIsDeleted == false && t.TaskIsDeleted == false && userBoards.Contains(b.KanbanBoardId)
                            select new { b.KanbanBoardId, t.TaskId, t.TaskCreatedAt, t.TaskName, t.TaskDescription, t.TaskStatus, t.TaskImageUrl };

            var userTasksResponse = await userTasks.Select(t => new TaskSearchResponse
            {
                TaskId = t.TaskId,
                TaskName = t.TaskName,
                TaskDescription = t.TaskDescription,
                TaskStatus = t.TaskStatus,
                TaskImage = t.TaskImageUrl,
                Link = $"/managetask/mytasks?b={t.KanbanBoardId}&t={t.TaskId}",
                CreatedAt = t.TaskCreatedAt,
            }).ToListAsync();

            //user teams
            var userTeams = await (from t in _dbContext.Team
                                   join p in _dbContext.Participation on t.TeamId equals p.ParticipationTeamId
                                   where p.ParticipationIsDeleted == false && p.ParticipationUserId == appSearchRequest.UserId
                                   select t.TeamId).Distinct().ToListAsync();

            //all boards of teams
            var allBoardsTeams = await (from kb in _dbContext.KanbanBoard
                                        where userTeams.Contains(kb.KanbanBoardTeamId)
                                        orderby kb.KanbanBoardCreatedAt descending
                                        select kb.KanbanBoardId).ToListAsync();

            //user tasks
            var teamTasks = from b in _dbContext.KanbanBoard.AsNoTracking()
                            join kl in _dbContext.KanbanList.AsNoTracking() on b.KanbanBoardId equals kl.KanbanListBoardBelongedId
                            join t in _dbContext.Task.AsNoTracking() on kl.KanbanListId equals t.TaskBelongedId
                            where kl.KanbanListIsDeleted == false && t.TaskIsDeleted == false && allBoardsTeams.Contains(b.KanbanBoardId)
                            select new { b.KanbanBoardTeamId, b.KanbanBoardId, t.TaskId, t.TaskCreatedAt, t.TaskName, t.TaskDescription, t.TaskStatus, t.TaskImageUrl };

            var teamTasksResponse = await teamTasks.Select(t => new TaskSearchResponse
            {
                TaskId = t.TaskId,
                TaskName = t.TaskName,
                TaskDescription = t.TaskDescription,
                TaskStatus = t.TaskStatus,
                TaskImage = t.TaskImageUrl,
                Link = $"/managetask/teamtasks?gr={t.KanbanBoardTeamId}&b={t.KanbanBoardId}&t={t.TaskId}",
                CreatedAt = t.TaskCreatedAt,
            }).ToListAsync();

            var response = new List<TaskSearchResponse>();
            response.AddRange(userTasksResponse);
            response.AddRange(teamTasksResponse);


            response = response.Where(r => r.TaskName.UnsignUnicode().Contains(appSearchRequest.KeyWord.UnsignUnicode())).ToList();
            var count = response.Count();

            response = response.OrderByDescending(t => t.CreatedAt)
                .Skip(appSearchRequest.SkipItems)
                .Take(appSearchRequest.PageSize)
                .ToList();

            return new PagedResponse<TaskSearchResponse>(response, appSearchRequest.PageSize, count, skipRows: appSearchRequest.SkipItems);
        }

        public async Task<PagedResponse<TeamSearchResponse>> SearchTeams(AppSearchRequest appSearchRequest)
        {
            var results = await (from t in _dbContext.Team.AsNoTracking()
                                 join p in _dbContext.Participation.AsNoTracking() on t.TeamId equals p.ParticipationTeamId
                                 where p.ParticipationIsDeleted == false && p.ParticipationUserId == appSearchRequest.UserId
                                 orderby t.TeamCreatedAt descending
                                 select new { t.TeamId, t.TeamName, t.TeamCode, t.TeamDescription, t.TeamImageUrl }).Distinct().ToListAsync();

            results = results.Where(r => r.TeamName.UnsignUnicode().Contains(appSearchRequest.KeyWord.UnsignUnicode())).ToList();
            var count = results.Count();

            var response = results.Select(r => new TeamSearchResponse
            {
                TeamId = r.TeamId,
                TeamName = r.TeamName,
                TeamDescription = r.TeamDescription,
                TeamCode = r.TeamCode,
                TeamImage = string.IsNullOrEmpty(r.TeamImageUrl) ? $"https://ui-avatars.com/api/?name={r.TeamName}" : r.TeamImageUrl,
                Link = $"team/{r.TeamId}",
            }).Skip(appSearchRequest.SkipItems).Take(appSearchRequest.PageSize).ToList();

            return new PagedResponse<TeamSearchResponse>(response, appSearchRequest.PageSize, count, skipRows: appSearchRequest.SkipItems);
        }
    }
}
