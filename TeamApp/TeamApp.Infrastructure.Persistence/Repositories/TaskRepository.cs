using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Task;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.Application.Utils;
using Microsoft.AspNetCore.SignalR;
using TeamApp.Infrastructure.Persistence.Hubs.Kanban;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IFileRepository _file;
        private readonly ICommentRepository _comment;
        private readonly IHubContext<HubKanbanClient, IHubKanbanClient> _hubKanban;

        public TaskRepository(TeamAppContext dbContext, IFileRepository file, ICommentRepository comment, IHubContext<HubKanbanClient, IHubKanbanClient> hubKanban)
        {
            _dbContext = dbContext;
            _file = file;
            _comment = comment;
            _hubKanban = hubKanban;
        }
        public async Task<string> AddTask(TaskRequest taskReq)
        {
            var entity = new Entities.Task
            {
                TaskId = Guid.NewGuid().ToString(),
                TaskName = taskReq.TaskName,
                TaskDescription = taskReq.TaskDescription,
                TaskPoint = taskReq.TaskPoint,
                TaskCreatedAt = DateTime.UtcNow,
                TaskStartDate = taskReq.TaskStartDate,
                TaskDeadline = taskReq.TaskDeadline,
                TaskStatus = taskReq.TaskStatus,
                TaskCompletedPercent = 0,
                TaskTeamId = taskReq.TaskTeamId,
                TaskIsDeleted = false,
                TaskBelongedId = taskReq.TaskBelongedId,
                TaskRankInList = taskReq.TaskRankInList,
                TaskThemeColor = taskReq.TaskThemeColor,
            };

            await _dbContext.Task.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            var taskUIKanban = new TaskUIKanban
            {
                RankInList = entity.TaskRankInList,
                KanbanListId = entity.TaskBelongedId,
                TaskId = entity.TaskId,
                TaskImageUrl = null,
                TaskName = entity.TaskName,
                TaskStartDate = entity.TaskStartDate,
                TaskDeadline = entity.TaskDeadline,
                TaskDescription = entity.TaskDescription,
                TaskStatus = entity.TaskStatus,
                CommentsCount = 0,
                FilesCount = 0,
                UserId = null,
                UserAvatar = null,
                TaskCompletedPercent = 0,
                TaskThemeColor = null,
            };

            var board = await (from kl in _dbContext.KanbanList.AsNoTracking()
                               join k in _dbContext.KanbanBoard.AsNoTracking() on kl.KanbanListBoardBelongedId equals k.KanbanBoardId
                               where kl.KanbanListId == taskReq.TaskBelongedId
                               select k).FirstOrDefaultAsync();

            var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                 join u in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals u.UserId
                                 where u.Type == "kanban" && (p.ParticipationTeamId == board.KanbanBoardTeamId || p.ParticipationUserId == board.KanbanBoardUserId)
                                 select u.ConnectionId).ToListAsync();

            await _hubKanban.Clients.Clients(clients).AddNewTask(taskUIKanban);

            return entity.TaskId;
        }

        public async Task<bool> DeleteTask(string taskId)
        {
            var entity = await _dbContext.Task.FindAsync(taskId);

            if (entity == null)
                return false;

            entity.TaskIsDeleted = true;
            _dbContext.Task.Update(entity);
            await _dbContext.SaveChangesAsync();

            var board = await (from kl in _dbContext.KanbanList.AsNoTracking()
                               join k in _dbContext.KanbanBoard.AsNoTracking() on kl.KanbanListBoardBelongedId equals k.KanbanBoardId
                               where kl.KanbanListId == entity.TaskBelongedId
                               select k).FirstOrDefaultAsync();

            var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                 join u in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals u.UserId
                                 where u.Type == "kanban" && p.ParticipationTeamId == board.KanbanBoardTeamId
                                 select u.ConnectionId).ToListAsync();

            await _hubKanban.Clients.Clients(clients).RemoveTask(new TaskRemoveModel
            {
                TaskId = taskId,
                KanbanListId = entity.TaskBelongedId,
            });

            return true;
        }

        public async Task<bool> DragTask(DragTaskModel dragTaskModel)
        {
            var entity = await _dbContext.Task.FindAsync(dragTaskModel.TaskId);

            if (entity == null)
                return false;
            var done = false;

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    entity.TaskRankInList = dragTaskModel.Position;
                    if (dragTaskModel.OldList != dragTaskModel.NewList)
                        entity.TaskBelongedId = dragTaskModel.NewList;
                    Console.WriteLine("Begin Transaction");
                    _dbContext.Task.Update(entity);
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
                var board = await (from b in _dbContext.KanbanBoard.AsNoTracking()
                                   join kl in _dbContext.KanbanList.AsNoTracking() on b.KanbanBoardId equals kl.KanbanListBoardBelongedId
                                   where kl.KanbanListId == dragTaskModel.NewList
                                   select b).FirstOrDefaultAsync();

                var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                     join u in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals u.UserId
                                     where u.Type == "kanban" && p.ParticipationTeamId == board.KanbanBoardTeamId
                                     select u.ConnectionId).ToListAsync();

                await _hubKanban.Clients.Clients(clients).MoveTask(dragTaskModel);
            }

            return true;
        }

        public async Task<List<TaskResponse>> GetAllByTeamId(string teamId)
        {
            var entityList = from ta in _dbContext.Task
                             where ta.TaskTeamId == teamId
                             select ta;

            var outPut = await entityList.Select(x => new TaskResponse
            {
                TaskId = x.TaskId,
                TaskName = x.TaskName,
                TaskDescription = x.TaskDescription,
                TaskPoint = x.TaskPoint,
                TaskCreatedAt = x.TaskCreatedAt.FormatTime(),
                TaskStartDate = x.TaskStartDate.FormatTime(),
                TaskDeadline = x.TaskDeadline.FormatTime(),
                TaskStatus = x.TaskStatus,
                TaskCompletedPercent = x.TaskCompletedPercent,
                TaskTeamId = x.TaskTeamId,
                TaskIsDeleted = x.TaskIsDeleted
            }).ToListAsync();

            return outPut;
        }

        public async Task<List<TaskResponse>> GetAllByUserId(string userId)
        {
            var entityList = from ta in _dbContext.Task
                             join p in _dbContext.Participation on ta.TaskTeamId equals p.ParticipationTeamId
                             join u in _dbContext.User on p.ParticipationUserId equals u.Id
                             select new { ta, u };

            entityList = entityList.Where(x => x.u.Id == userId);

            var outPut = await entityList.Select(x => new TaskResponse
            {
                TaskId = x.ta.TaskId,
                TaskName = x.ta.TaskName,
                TaskDescription = x.ta.TaskDescription,
                TaskPoint = x.ta.TaskPoint,
                TaskCreatedAt = x.ta.TaskCreatedAt.FormatTime(),
                TaskStartDate = x.ta.TaskStartDate.FormatTime(),
                TaskDeadline = x.ta.TaskDeadline.FormatTime(),
                TaskStatus = x.ta.TaskStatus,
                TaskCompletedPercent = x.ta.TaskCompletedPercent,
                TaskTeamId = x.ta.TaskTeamId,
                TaskIsDeleted = x.ta.TaskIsDeleted
            }).ToListAsync();

            return outPut;
        }

        public async Task<List<TaskResponse>> GetAllByUserTeamId(string userId, string teamId)
        {
            var entityList = from ta in _dbContext.Task
                             join p in _dbContext.Participation on ta.TaskTeamId equals p.ParticipationTeamId
                             join u in _dbContext.User on p.ParticipationUserId equals u.Id
                             select new { ta, u };

            entityList = entityList.Where(x => x.u.Id == userId && x.ta.TaskTeamId == teamId);

            var outPut = await entityList.Select(x => new TaskResponse
            {
                TaskId = x.ta.TaskId,
                TaskName = x.ta.TaskName,
                TaskDescription = x.ta.TaskDescription,
                TaskPoint = x.ta.TaskPoint,
                TaskCreatedAt = x.ta.TaskCreatedAt.FormatTime(),
                TaskStartDate = x.ta.TaskStartDate.FormatTime(),
                TaskDeadline = x.ta.TaskDeadline.FormatTime(),
                TaskStatus = x.ta.TaskStatus,
                TaskCompletedPercent = x.ta.TaskCompletedPercent,
                TaskTeamId = x.ta.TaskTeamId,
                TaskIsDeleted = x.ta.TaskIsDeleted
            }).ToListAsync();

            return outPut;
        }

        public async Task<TaskResponse> GetById(string taskId)
        {
            var query = from t in _dbContext.Task.AsNoTracking()
                        join h in _dbContext.HandleTask.AsNoTracking() on t.TaskId equals h.HandleTaskTaskId into tHandle

                        from th in tHandle.DefaultIfEmpty()
                        join u in _dbContext.User.AsNoTracking() on th.HandleTaskUserId equals u.Id into tUser

                        from tu in tUser.DefaultIfEmpty()
                        where t.TaskId == taskId
                        select new { t, tu.FullName, tu.Id, tu.ImageUrl };

            var listComments = await _comment.GetListByTask(taskId);
            var listFiles = await _file.GetAllByTask(taskId);

            var task = await query.AsNoTracking().FirstOrDefaultAsync();

            if (task == null)
                return null;
            var outPut = new TaskResponse
            {
                KanbanListId = task.t.TaskBelongedId,
                TaskId = task.t.TaskId,
                TaskName = task.t.TaskName,
                TaskDescription = task.t.TaskDescription,
                TaskPoint = task.t.TaskPoint,
                TaskCreatedAt = task.t.TaskCreatedAt.FormatTime(),
                TaskStartDate = task.t.TaskStartDate.FormatTime(),
                TaskDeadline = task.t.TaskDeadline.FormatTime(),
                TaskStatus = task.t.TaskStatus,
                TaskCompletedPercent = task.t.TaskCompletedPercent,
                TaskTeamId = task.t.TaskTeamId,
                TaskIsDeleted = task.t.TaskIsDeleted,
                TaskThemeColor = task.t.TaskThemeColor,
                UserId = task.Id,
                UserName = task.FullName,
                UserAvatar = task.ImageUrl,
                RankInList = task.t.TaskRankInList,
                Comments = listComments,
                Files = listFiles,
                TaskImageUrl = task.t.TaskImageUrl,
            };

            return outPut;
        }

        public async Task<PagedResponse<TaskResponse>> GetPaging(RequestParameter parameter)
        {
            var query = _dbContext.Task.Skip(parameter.PageSize * parameter.PageNumber).Take(parameter.PageSize);

            var entityList = await query.Select(x => new TaskResponse
            {
                TaskId = x.TaskId,
                TaskName = x.TaskName,
                TaskDescription = x.TaskDescription,
                TaskPoint = x.TaskPoint,
                TaskCreatedAt = x.TaskCreatedAt.FormatTime(),
                TaskStartDate = x.TaskStartDate.FormatTime(),
                TaskDeadline = x.TaskDeadline.FormatTime(),
                TaskStatus = x.TaskStatus,
                TaskCompletedPercent = x.TaskCompletedPercent,
                TaskTeamId = x.TaskTeamId,
                TaskIsDeleted = x.TaskIsDeleted,
            }).ToListAsync();

            var outPut = new PagedResponse<TaskResponse>(entityList, parameter.PageNumber, parameter.PageSize, await query.CountAsync());

            return outPut;
        }

        public async Task<bool> UpdateTask(TaskUpdateRequest taskReq)
        {
            var entity = await _dbContext.Task.FindAsync(taskReq.TaskId);
            if (entity == null)
                return false;

            entity.TaskDescription = taskReq.TaskDescription == null ? entity.TaskDescription : taskReq.TaskDescription;
            entity.TaskName = taskReq.TaskName == null ? entity.TaskName : taskReq.TaskName;

            entity.TaskThemeColor = taskReq.TaskThemeColor;
            entity.TaskStartDate = taskReq.TaskStartDate;
            entity.TaskStatus = taskReq.TaskStatus;
            entity.TaskCompletedPercent = taskReq.TaskCompletedPercent;
            entity.TaskImageUrl = taskReq.TaskImageUrl;
            entity.TaskDeadline = taskReq.TaskDeadline;

            entity.TaskPoint = taskReq.TaskPoint == null ? entity.TaskPoint : taskReq.TaskPoint;
            entity.TaskCreatedAt = taskReq.TaskCreatedAt == null ? entity.TaskCreatedAt : taskReq.TaskCreatedAt;


            //entity.TaskTeamId = taskReq.TaskTeamId == null ? entity.TaskTeamId : taskReq.TaskTeamId;
            //entity.TaskIsDeleted = taskReq.TaskIsDeleted == null ? entity.TaskIsDeleted : taskReq.TaskIsDeleted;

            _dbContext.Task.Update(entity);
            var check = await _dbContext.SaveChangesAsync() > 0;

            return check;
        }
    }
}
