﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanBoard;
using TeamApp.Application.DTOs.KanbanList;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using TeamApp.Application.DTOs.Task;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.Utils;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using TeamApp.Infrastructure.Persistence.Hubs.Kanban;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class KanbanBoardRepository : IKanbanBoardRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IHubContext<HubKanbanClient, IHubKanbanClient> _hubKanban;
        public KanbanBoardRepository(TeamAppContext teamAppContext, IHubContext<HubKanbanClient, IHubKanbanClient> hubKanban)
        {
            _dbContext = teamAppContext;
            _hubKanban = hubKanban;
        }

        public async Task<KanbanBoardResponse> AddKanbanBoard(KanbanBoardRequest kanbanBoardRequest)
        {
            var entity = new KanbanBoard
            {
                KanbanBoardId = string.IsNullOrEmpty(kanbanBoardRequest.KanbanBoardId) ? Guid.NewGuid().ToString() : kanbanBoardRequest.KanbanBoardId,
                KanbanBoardIsOfTeam = kanbanBoardRequest.KanbanBoardIsOfTeam,
                KanbanBoardUserId = kanbanBoardRequest.KanbanBoardUserId,
                KanbanBoardTeamId = kanbanBoardRequest.KanbanBoardTeamId,
                KanbanBoardName = kanbanBoardRequest.KanbanBoardName,
                KanbanBoardCreatedAt = DateTime.UtcNow,
            };

            await _dbContext.KanbanBoard.AddAsync(entity);
            var check = await _dbContext.SaveChangesAsync() > 0;

            if (check)
            {
                return new KanbanBoardResponse
                {
                    KanbanBoardId = entity.KanbanBoardId,
                    KanbanBoardIsOfTeam = entity.KanbanBoardIsOfTeam,
                    KanbanBoardUserId = kanbanBoardRequest.KanbanBoardUserId,
                    KanbanBoardTeamId = kanbanBoardRequest.KanbanBoardTeamId,
                    KanbanBoardName = entity.KanbanBoardName,
                    TaskCount = 0,
                };
            }
            return null;
        }

        public async Task<List<KanbanBoardResponse>> GetBoardForUserTeams(string userId)
        {
            //get all team for user
            var teams = await (from p in _dbContext.Participation.AsNoTracking()
                               join t in _dbContext.Team.AsNoTracking() on p.ParticipationTeamId equals t.TeamId
                               join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                               where u.Id == userId
                               select t).ToListAsync();

            List<KanbanBoardResponse> responses = new List<KanbanBoardResponse>();

            foreach (var team in teams)
            {
                var boards = await (from b in _dbContext.KanbanBoard.AsNoTracking()
                                    where b.KanbanBoardTeamId == team.TeamId
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
                        TaskCount = taskCount,
                        KanbanBoardGroupName = team.TeamName,
                        GroupImageUrl = string.IsNullOrEmpty(team.TeamImageUrl) ? $"https://ui-avatars.com/api/?name={team.TeamName}" : team.TeamImageUrl,
                    });
                }
            }

            return responses;
        }

        public async Task<List<KanbanBoardResponse>> GetBoardForUser(string userId)
        {
            var list = from kb in _dbContext.KanbanBoard.AsNoTracking()
                       where kb.KanbanBoardUserId == userId
                       orderby kb.KanbanBoardCreatedAt
                       select kb;
            if (list == null)
                return null;
            var listBoards = await list.ToListAsync();

            Dictionary<string, int> TaskCounts = new Dictionary<string, int>();

            foreach (var lb in listBoards)
            {
                var taskCount = await (from t in _dbContext.Task.AsNoTracking()
                                       join kl in _dbContext.KanbanList.AsNoTracking() on t.TaskBelongedId equals kl.KanbanListId
                                       where kl.KanbanListBoardBelongedId == lb.KanbanBoardId && t.TaskIsDeleted == false
                                       select t.TaskId).CountAsync();

                TaskCounts.Add(lb.KanbanBoardId, taskCount);
            }


            return await list.Select(x => new KanbanBoardResponse
            {
                KanbanBoardId = x.KanbanBoardId,
                KanbanBoardUserId = x.KanbanBoardUserId,
                KanbanBoardTeamId = x.KanbanBoardTeamId,
                KanbanBoardName = x.KanbanBoardName,
                TaskCount = TaskCounts[x.KanbanBoardId]
            }).ToListAsync();
        }

        public async Task<KanbanBoardUIResponse> GetKanbanBoardUI(string boardId)
        {
            var board = await (from b in _dbContext.KanbanBoard.AsNoTracking()
                               where b.KanbanBoardId == boardId
                               select b).AsNoTracking().FirstOrDefaultAsync();
            if (board == null)
                return null;

            var taskListUIs = new List<TaskUIKanban>();
            var outPut = new KanbanBoardUIResponse
            {
                KanbanBoardId = board.KanbanBoardId,
                KanbanBoardUserId = board.KanbanBoardUserId,
                KanbanBoardTeamId = board.KanbanBoardTeamId,
                KanbanBoardIsOfTeam = board.KanbanBoardIsOfTeam,
                KanbanListUIs = new List<KanbanListUIResponse>(),
            };

            //danh sach kanbanlist cua 1 board
            var listKanbanQuery = from kl in _dbContext.KanbanList.AsNoTracking()
                                  where kl.KanbanListBoardBelongedId == boardId && !kl.KanbanListIsDeleted.Value
                                  orderby kl.KanbanListOrderInBoard
                                  select kl;

            var listKanban = await listKanbanQuery.AsNoTracking().ToListAsync();
            var listKanbanArray = listKanban.Select(x => x.KanbanListId).ToArray();


            var listTasksQuery = from t in _dbContext.Task.AsNoTracking()
                                 join h in _dbContext.HandleTask.AsNoTracking() on t.TaskId equals h.HandleTaskTaskId into tHandle

                                 from th in tHandle.DefaultIfEmpty()
                                 join u in _dbContext.User.AsNoTracking() on th.HandleTaskUserId equals u.Id into tUser

                                 from tu in tUser.DefaultIfEmpty()
                                 where listKanbanArray.Contains(t.TaskBelongedId) && t.TaskIsDeleted == false

                                 select new { t, tu.ImageUrl, tu.Id, t.Comments };

            var listTasks = await listTasksQuery.AsNoTracking().Where(x => x.t.TaskIsDeleted == false).ToListAsync();

            foreach (var kl in listKanban)
            {
                var kanbanListTasks = listTasks.Where(x => x.t.TaskBelongedId == kl.KanbanListId).OrderBy(x => x.t.TaskOrderInList);

                var listUITasks = kanbanListTasks.Select(x =>
                         new TaskUIKanban
                         {
                             OrderInList = x.t.TaskOrderInList,
                             KanbanListId = x.t.TaskBelongedId,
                             TaskId = x.t.TaskId,

                             TaskName = x.t.TaskName,
                             TaskStartDate = x.t.TaskStartDate.FormatTime(),
                             TaskDeadline = x.t.TaskDeadline.FormatTime(),
                             TaskStatus = x.t.TaskStatus,
                             TaskDescription = x.t.TaskDescription,

                             TaskImageUrl = x.t.TaskImageUrl,

                             CommentsCount = x.Comments.Count,
                             FilesCount = _dbContext.File.AsNoTracking().Where(f => f.FileTaskOwnerId == x.t.TaskId).Count(),

                             UserId = x.Id,
                             UserAvatar = x.ImageUrl,

                             TaskCompletedPercent = x.t.TaskCompletedPercent,

                             TaskThemeColor = x.t.TaskThemeColor,
                         }
                    ).ToList();

                var kbListUi = new KanbanListUIResponse
                {
                    KanbanListId = kl.KanbanListId,
                    KanbanListTitle = kl.KanbanListTitle,
                    KanbanListBoardBelongedId = kl.KanbanListBoardBelongedId,
                    KanbanListOrderInBoard = kl.KanbanListOrderInBoard,
                    TaskUIKanbans = listUITasks,
                };

                outPut.KanbanListUIs.Add(kbListUi);
            }
            /*var listKb = await listKanban.ToListAsync();

            //duyet tat ca cac kanbanlist de lay ra danh sach task cua moi list
            foreach (var kl in listKb)
            {
                var tasklist = from t in _dbContext.Task.AsNoTracking()
                               join h in _dbContext.HandleTask.AsNoTracking() on t.TaskId equals h.HandleTaskTaskId
                               join u in _dbContext.User.AsNoTracking() on h.HandleTaskUserId equals u.Id
                               where t.TaskBelongedId == kl.KanbanListId && t.TaskIsDeleted == false
                               select new { t, u.ImageUrl, u.Id };

                //tasks of 1 listkanban
                var taskLists = await tasklist.AsNoTracking().OrderBy(x => x.t.TaskOrderInList).ToListAsync();

                taskListUI = new List<TaskUIKanban>();

                //cac task trong list kanban
                foreach (var x in taskLists)
                {
                    var files = await _dbContext.File.AsNoTracking().Where(f => f.FileBelongedId == x.t.TaskId).CountAsync();

                    var taskUIKanban = new TaskUIKanban
                    {
                        OrderInList = x.t.TaskOrderInList,
                        KanbanListId = x.t.TaskBelongedId,
                        TaskId = x.t.TaskId,

                        TaskName = x.t.TaskName,
                        TaskDeadline = x.t.TaskDeadline.FormatTime(),
                        TaskStatus = x.t.TaskStatus,
                        TaskDescription = x.t.TaskDescription,

                        Image = x.t.TaskImageUrl,

                        CommentsCount = x.t.Comments.Count,
                        FilesCount = files,

                        UserId = x.Id,
                        UserAvatar = x.ImageUrl,

                        TaskCompletedPercent = x.t.TaskCompletedPercent,
                    };

                    taskListUI.Add(taskUIKanban);
                }

                var kbListUi = new KanbanListUIResponse
                {
                    KanbanListId = kl.KanbanListId,
                    KanbanListTitle = kl.KanbanListTitle,
                    KanbanListBoardBelongedId = kl.KanbanListBoardBelongedId,
                    KanbanListOrderInBoard = kl.KanbanListOrderInBoard,
                    TaskUIKanbans = taskListUI,
                };

                outPut.KanbanListUIs.Add(kbListUi);
            }*/
            return outPut;
        }

        public Task<List<KanbanListUIResponse>> GetKanbanList(string boardId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SwapListKanban(SwapListModel swapListModel)
        {
            var kblEntity = await _dbContext.KanbanList.Where(x => x.KanbanListId == swapListModel.KanbanListId && x.KanbanListIsDeleted == false).FirstOrDefaultAsync();

            if (kblEntity == null)
                return false;

            var done = false;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    kblEntity.KanbanListOrderInBoard = swapListModel.Position;
                    Console.WriteLine("Begin Transaction");
                    _dbContext.KanbanList.Update(kblEntity);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    done = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            if (done)
            {
                var board = await _dbContext.KanbanBoard.FindAsync(swapListModel.KanbanBoardId);

                var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                     join u in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals u.UserId
                                     where u.Type == "kanban" && p.ParticipationTeamId == board.KanbanBoardTeamId
                                     select u.ConnectionId).ToListAsync();

                await _hubKanban.Clients.Clients(clients).MoveList(swapListModel);
            }

            return true;
        }

        public async Task<List<KanbanBoardResponse>> GetBoardsForTeam(string teamId)
        {
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
                    TaskCount = taskCount,
                });
            }

            return responses;
        }
    }
}
