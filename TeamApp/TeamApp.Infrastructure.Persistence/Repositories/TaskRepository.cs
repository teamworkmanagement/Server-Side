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
using TeamApp.Application.DTOs.TaskVersion;
using Newtonsoft.Json;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IFileRepository _file;
        private readonly ICommentRepository _comment;
        private readonly IHubContext<HubKanbanClient, IHubKanbanClient> _hubKanban;
        private readonly ITaskVersionRepository _taskVersionRepository;
        private readonly ITeamRepository _teamRepository;

        public TaskRepository(TeamAppContext dbContext, IFileRepository file, ICommentRepository comment, IHubContext<HubKanbanClient, IHubKanbanClient> hubKanban, ITaskVersionRepository taskVersionRepository, ITeamRepository teamRepository)
        {
            _dbContext = dbContext;
            _file = file;
            _comment = comment;
            _hubKanban = hubKanban;
            _taskVersionRepository = taskVersionRepository;
            _teamRepository = teamRepository;
        }

        TaskVersionRequest MapTaskToTaskVersionRequest(Entities.Task task, string actionUserId)
        {
            return new TaskVersionRequest
            {
                TaskVersionTaskId = task.TaskId,
                TaskVersionUpdatedAt = DateTime.UtcNow,
                TaskVersionTaskName = task.TaskName,
                TaskVersionTaskDescription = task.TaskDescription,
                TaskVersionTaskPoint = task.TaskPoint,
                TaskVersionTaskDeadline = task.TaskDeadline,
                TaskVersionStartDate = task.TaskStartDate,
                TaskVersionDoneDate = task.TaskDoneDate,
                TaskVersionTaskStatus = task.TaskStatus,
                TaskVersionTaskCompletedPercent = task.TaskCompletedPercent,
                TaskVersionActionUserId = actionUserId,
            };
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
                TaskRankInList = entity.TaskRankInList,
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
                                 where u.Type == "kanban" && ((p.ParticipationTeamId == board.KanbanBoardTeamId && p.ParticipationIsDeleted == false) || p.ParticipationUserId == board.KanbanBoardUserId)
                                 select u.ConnectionId).Distinct().ToListAsync();

            await _hubKanban.Clients.Clients(clients).AddNewTask(taskUIKanban);

            var taskVersion = MapTaskToTaskVersionRequest(entity, taskReq.UserActionId);
            await _taskVersionRepository.AddTaskVersion(taskVersion);

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
                                 where u.Type == "kanban" && ((p.ParticipationTeamId == board.KanbanBoardTeamId && p.ParticipationIsDeleted == false) || p.ParticipationUserId == board.KanbanBoardUserId)
                                 select u.ConnectionId).Distinct().ToListAsync();

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
                                     where u.Type == "kanban" && ((p.ParticipationTeamId == board.KanbanBoardTeamId && p.ParticipationIsDeleted == false) || p.ParticipationUserId == board.KanbanBoardUserId) && u.ConnectionId != dragTaskModel.ConnectionId
                                     select u.ConnectionId).Distinct().ToListAsync();

                await _hubKanban.Clients.Clients(clients).MoveTask(dragTaskModel);
            }

            return true;
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

            if (task == null || task.t.TaskIsDeleted.Value)
                throw new KeyNotFoundException("Not found task");
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
                UserAvatar = string.IsNullOrEmpty(task.ImageUrl) ? $"https://ui-avatars.com/api/?name={task.FullName}" : task.ImageUrl,
                RankInList = task.t.TaskRankInList,
                Comments = listComments,
                Files = listFiles,
                TaskImageUrl = task.t.TaskImageUrl,
            };

            return outPut;
        }

        public async Task<TaskResponse> GetTaskByBoard(string userId, TaskGetRequest taskGetRequest)
        {
            var query = from t in _dbContext.Task.AsNoTracking()
                        join h in _dbContext.HandleTask.AsNoTracking() on t.TaskId equals h.HandleTaskTaskId into tHandle

                        from th in tHandle.DefaultIfEmpty()
                        join u in _dbContext.User.AsNoTracking() on th.HandleTaskUserId equals u.Id into tUser

                        from tu in tUser.DefaultIfEmpty()
                        where t.TaskId == taskGetRequest.TaskId && t.TaskIsDeleted == false
                        select new { t, tu.FullName, tu.Id, tu.ImageUrl, t.Comments.Count };

            var task = await query.AsNoTracking().FirstOrDefaultAsync();

            if (task == null || task.t.TaskIsDeleted.Value)
                throw new KeyNotFoundException("Not found task");

            var kl = await _dbContext.KanbanList.FindAsync(task.t.TaskBelongedId);

            var kb = await _dbContext.KanbanBoard.FindAsync(kl.KanbanListBoardBelongedId);

            if (kb == null)
                throw new KeyNotFoundException("Not found");

            else
            {
                if (kb.KanbanBoardId != taskGetRequest.BoardId)
                    throw new KeyNotFoundException("Not found");
            }


            if (taskGetRequest.IsOfTeam)
            {
                if (kb.KanbanBoardTeamId != taskGetRequest.OwnerId)
                    throw new KeyNotFoundException("Board not found");

                var memberCheck = await (from p in _dbContext.Participation.AsNoTracking()
                                         where p.ParticipationIsDeleted == false && p.ParticipationUserId == userId && p.ParticipationTeamId == kb.KanbanBoardTeamId
                                         select p).FirstOrDefaultAsync();
                if (memberCheck == null)
                    throw new KeyNotFoundException("Not found permission");
            }
            else
            {
                if (kb.KanbanBoardUserId != taskGetRequest.OwnerId)
                    throw new KeyNotFoundException("Board not found");
            }


            var listComments = await _comment.GetListByTask(taskGetRequest.TaskId);
            var listFiles = await _file.GetAllByTask(taskGetRequest.TaskId);

            bool showPoint = false;
            if (!string.IsNullOrEmpty(kb.KanbanBoardTeamId))
            {
                var admin = await _teamRepository.GetAdmin(kb.KanbanBoardTeamId, userId);
                if (admin.UserId == taskGetRequest.UserRequest)
                    showPoint = true;
            }
            else
            {
                showPoint = true;
            }


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
                ShowPoint = showPoint,
                CommentsCount = task.Count,
            };

            return outPut;
        }

        public object GetTaskUIKanban(Entities.Task entity) => new
        {
            entity.TaskRankInList,
            KanbanListId = entity.TaskBelongedId,
            entity.TaskId,

            entity.TaskName,
            TaskStartDate = entity.TaskStartDate.FormatTime(),
            TaskDeadline = entity.TaskDeadline.FormatTime(),
            entity.TaskStatus,
            entity.TaskDescription,

            entity.TaskImageUrl,

            CommentsCount = _dbContext.Comment.AsNoTracking().Where(c => c.CommentTaskId == entity.TaskId).Count(),
            FilesCount = _dbContext.File.AsNoTracking().Where(f => f.FileTaskOwnerId == entity.TaskId).Count(),

            entity.TaskCompletedPercent,

            entity.TaskThemeColor,
        };

        public async Task<bool> UpdateTask(TaskUpdateRequest taskReq)
        {
            var entity = await _dbContext.Task.FindAsync(taskReq.TaskId);
            if (entity == null)
                throw new KeyNotFoundException("Task not found");

            var oldTaskStr = JsonConvert.SerializeObject(entity);

            entity.TaskDescription = taskReq.TaskDescription ?? entity.TaskDescription;
            entity.TaskName = taskReq.TaskName ?? entity.TaskName;

            entity.TaskThemeColor = taskReq.TaskThemeColor;
            entity.TaskStartDate = taskReq.TaskStartDate;
            entity.TaskStatus = taskReq.TaskStatus;
            entity.TaskCompletedPercent = taskReq.TaskCompletedPercent;
            entity.TaskImageUrl = taskReq.TaskImageUrl;
            entity.TaskDeadline = taskReq.TaskDeadline;

            entity.TaskPoint = taskReq.TaskPoint ?? entity.TaskPoint;
            entity.TaskCreatedAt = taskReq.TaskCreatedAt ?? entity.TaskCreatedAt;

            if (entity.TaskStatus == "done")
                entity.TaskDoneDate = DateTime.UtcNow;
            else
                entity.TaskDoneDate = null;
            //entity.TaskTeamId = taskReq.TaskTeamId == null ? entity.TaskTeamId : taskReq.TaskTeamId;
            //entity.TaskIsDeleted = taskReq.TaskIsDeleted == null ? entity.TaskIsDeleted : taskReq.TaskIsDeleted;

            _dbContext.Task.Update(entity);
            var check = await _dbContext.SaveChangesAsync() > 0;

            if (check)
            {
                var board = await (from b in _dbContext.KanbanBoard.AsNoTracking()
                                   join kl in _dbContext.KanbanList.AsNoTracking() on b.KanbanBoardId equals kl.KanbanListBoardBelongedId
                                   where kl.KanbanListId == entity.TaskBelongedId
                                   select b).FirstOrDefaultAsync();

                var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                     join u in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals u.UserId
                                     where u.Type == "kanban" && ((p.ParticipationTeamId == board.KanbanBoardTeamId && p.ParticipationIsDeleted == false) || p.ParticipationUserId == board.KanbanBoardUserId)
                                     select u.ConnectionId).Distinct().ToListAsync();

                await _hubKanban.Clients.Clients(clients).UpdateTask(GetTaskUIKanban(entity));
            }

            var oldTask = JsonConvert.DeserializeObject<Entities.Task>(oldTaskStr);

            if (oldTask.TaskName != entity.TaskName || oldTask.TaskDescription != entity.TaskDescription
                || oldTask.TaskDeadline != entity.TaskDeadline || oldTask.TaskStatus != entity.TaskStatus
                || oldTask.TaskCompletedPercent != entity.TaskCompletedPercent || oldTask.TaskPoint != entity.TaskPoint
                )
            {
                var taskVersion = MapTaskToTaskVersionRequest(entity, taskReq.UserActionId);
                await _taskVersionRepository.AddTaskVersion(taskVersion);
            }


            return check;
        }
    }
}
