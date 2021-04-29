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

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IFileRepository _file;
        private readonly ICommentRepository _comment;

        public TaskRepository(TeamAppContext dbContext, IFileRepository file, ICommentRepository comment)
        {
            _dbContext = dbContext;
            _file = file;
            _comment = comment;
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
                TaskDeadline = taskReq.TaskDeadline,
                TaskStatus = taskReq.TaskStatus,
                TaskCompletedPercent = taskReq.TaskCompletedPercent,
                TaskTeamId = taskReq.TaskTeamId,
                TaskIsDeleted = false,
                TaskBelongedId = taskReq.TaskBelongedId,
                TaskOrderInList = taskReq.TaskOrderInList,
                TaskThemeColor = taskReq.TaskThemeColor,
            };

            await _dbContext.Task.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
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
            var query = from t in _dbContext.Task
                        join h in _dbContext.HandleTask on t.TaskId equals h.HandleTaskTaskId
                        join u in _dbContext.User on h.HandleTaskUserId equals u.Id
                        where t.TaskId == taskId
                        select new { t, u.FullName, u.Id, u.ImageUrl };

            var listComments = await _comment.GetListByTask(taskId);
            var listFiles = await _file.GetAllByTask(taskId);

            var task = await query.FirstOrDefaultAsync();

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
                TaskDeadline = task.t.TaskDeadline.FormatTime(),
                TaskStatus = task.t.TaskStatus,
                TaskCompletedPercent = task.t.TaskCompletedPercent,
                TaskTeamId = task.t.TaskTeamId,
                TaskIsDeleted = task.t.TaskIsDeleted,
                TaskThemeColor = task.t.TaskThemeColor,
                UserId = task.Id,
                UserName = task.FullName,
                UserAvatar = task.ImageUrl,
                OrderInList = task.t.TaskOrderInList,
                Comments = listComments,
                Files = listFiles,
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
                TaskDeadline = x.TaskDeadline.FormatTime(),
                TaskStatus = x.TaskStatus,
                TaskCompletedPercent = x.TaskCompletedPercent,
                TaskTeamId = x.TaskTeamId,
                TaskIsDeleted = x.TaskIsDeleted,
            }).ToListAsync();

            var outPut = new PagedResponse<TaskResponse>(entityList, parameter.PageNumber, parameter.PageSize, await query.CountAsync());

            return outPut;
        }

        public async Task<bool> UpdateTask(string taskId, TaskRequest taskReq)
        {
            var entity = await _dbContext.Task.FindAsync(taskId);
            if (entity == null)
                return false;

            entity.TaskName = taskReq.TaskName;
            entity.TaskDeadline = taskReq.TaskDeadline;
            entity.TaskPoint = taskReq.TaskPoint;
            entity.TaskCreatedAt = taskReq.TaskCreatedAt;
            entity.TaskDeadline = taskReq.TaskDeadline;
            entity.TaskStatus = taskReq.TaskStatus;
            entity.TaskCompletedPercent = taskReq.TaskCompletedPercent;
            entity.TaskTeamId = taskReq.TaskTeamId;
            entity.TaskIsDeleted = taskReq.TaskIsDeleted;

            _dbContext.Task.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
