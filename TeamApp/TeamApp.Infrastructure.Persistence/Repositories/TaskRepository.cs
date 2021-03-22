using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Models.Task;
using TeamApp.Infrastructure.Persistence.Entities;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly KhoaLuanContext _dbContext;

        public TaskRepository(KhoaLuanContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> AddTask(TaskRequest taskReq)
        {
            var entity = new Entities.Task
            {
                TaskId = new Guid().ToString(),
                TaskName = taskReq.TaskName,
                TaskDescription = taskReq.TaskDescription,
                TaskPoint = taskReq.TaskPoint,
                TaskCreatedAt = DateTime.UtcNow,
                TaskDeadline = taskReq.TaskDeadline,
                TaskStatus = taskReq.TaskStatus,
                TaskCompletedPercent = taskReq.TaskCompletedPercent,
                TaskTeamId = taskReq.TaskTeamId,
                TaskIsDeleted = false
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
                TaskCreatedAt = x.TaskCreatedAt,
                TaskDeadline = x.TaskDeadline,
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
                             join u in _dbContext.User on p.ParticipationUserId equals u.UserId
                             select new { ta, u };

            entityList = entityList.Where(x => x.u.UserId == userId);

            var outPut = await entityList.Select(x => new TaskResponse
            {
                TaskId = x.ta.TaskId,
                TaskName = x.ta.TaskName,
                TaskDescription = x.ta.TaskDescription,
                TaskPoint = x.ta.TaskPoint,
                TaskCreatedAt = x.ta.TaskCreatedAt,
                TaskDeadline = x.ta.TaskDeadline,
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
                             join u in _dbContext.User on p.ParticipationUserId equals u.UserId
                             select new { ta, u };

            entityList = entityList.Where(x => x.u.UserId == userId && x.ta.TaskTeamId == teamId);

            var outPut = await entityList.Select(x => new TaskResponse
            {
                TaskId = x.ta.TaskId,
                TaskName = x.ta.TaskName,
                TaskDescription = x.ta.TaskDescription,
                TaskPoint = x.ta.TaskPoint,
                TaskCreatedAt = x.ta.TaskCreatedAt,
                TaskDeadline = x.ta.TaskDeadline,
                TaskStatus = x.ta.TaskStatus,
                TaskCompletedPercent = x.ta.TaskCompletedPercent,
                TaskTeamId = x.ta.TaskTeamId,
                TaskIsDeleted = x.ta.TaskIsDeleted
            }).ToListAsync();

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
                TaskCreatedAt = x.TaskCreatedAt,
                TaskDeadline = x.TaskDeadline,
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
