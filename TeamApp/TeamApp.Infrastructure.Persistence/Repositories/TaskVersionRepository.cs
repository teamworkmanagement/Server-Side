using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.DTOs.TaskVersion;
using TeamApp.Application.Utils;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TaskVersionRepository : ITaskVersionRepository
    {
        private readonly TeamAppContext _dbContext;

        public TaskVersionRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> AddTaskVersion(TaskVersionRequest taskVersionRequest)
        {
            var entity = new TaskVersion
            {
                TaskVersionId = Guid.NewGuid().ToString(),
                TaskVersionTaskId = taskVersionRequest.TaskVersionTaskId,
                TaskVersionUpdatedAt = DateTime.UtcNow,
                TaskVersionTaskName = taskVersionRequest.TaskVersionTaskName,
                TaskVersionTaskDescription = taskVersionRequest.TaskVersionTaskDescription,
                TaskVersionTaskPoint = taskVersionRequest.TaskVersionTaskPoint,
                TaskVersionStartDate = taskVersionRequest.TaskVersionStartDate,
                TaskVersionTaskDeadline = taskVersionRequest.TaskVersionTaskDeadline,
                TaskVersionDoneDate = taskVersionRequest.TaskVersionDoneDate,
                TaskVersionTaskStatus = taskVersionRequest.TaskVersionTaskStatus,
                TaskVersionTaskCompletedPercent = taskVersionRequest.TaskVersionTaskCompletedPercent,
                TaskVersionActionUserId = taskVersionRequest.TaskVersionActionUserId,
            };

            await _dbContext.TaskVersion.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.TaskVersionId;
        }

        public async Task<List<TaskVersionResponse>> GetAllByTaskId(string taskId)
        {
            var task = await _dbContext.Task.FindAsync(taskId);
            if (task == null)
                throw new KeyNotFoundException("Not found task");

            var query = from tv in _dbContext.TaskVersion.AsNoTracking()
                        join u in _dbContext.User.AsNoTracking() on tv.TaskVersionActionUserId equals u.Id
                        where tv.TaskVersionTaskId == taskId
                        orderby tv.TaskVersionUpdatedAt descending
                        select new { tv, u };

            return await query.Select(x => new TaskVersionResponse
            {
                TaskVersionId = x.tv.TaskVersionId,
                TaskVersionTaskId = x.tv.TaskVersionTaskId,
                TaskVersionUpdatedAt = x.tv.TaskVersionUpdatedAt.FormatTime(),
                TaskVersionTaskName = x.tv.TaskVersionTaskName,
                TaskVersionTaskDescription = x.tv.TaskVersionTaskDescription,
                TaskVersionTaskPoint = x.tv.TaskVersionTaskPoint,
                TaskVersionTaskDeadline = x.tv.TaskVersionTaskDeadline,
                TaskVersionStartDate = x.tv.TaskVersionStartDate.FormatTime(),
                TaskVersionDoneDate = x.tv.TaskVersionDoneDate.FormatTime(),
                TaskVersionTaskStatus = x.tv.TaskVersionTaskStatus,
                TaskVersionTaskCompletedPercent = x.tv.TaskVersionTaskCompletedPercent,
                TaskVersionActionUserId = x.u.Id,
                TaskVersionActionUserName = x.u.FullName,
                TaskVersionActionUserImage = string.IsNullOrEmpty(x.u.ImageUrl) ? $"https://ui-avatars.com/api/?name={x.u.FullName}" : x.u.ImageUrl
            }).ToListAsync();
        }

        public async Task<TaskVersionResponse> GetById(string taskVerId)
        {
            var entity = await _dbContext.TaskVersion.FindAsync(taskVerId);

            if (entity == null)
                return null;

            return new TaskVersionResponse
            {
                TaskVersionId = entity.TaskVersionId,
                TaskVersionTaskId = entity.TaskVersionTaskId,
                TaskVersionUpdatedAt = entity.TaskVersionUpdatedAt.FormatTime(),
                TaskVersionTaskName = entity.TaskVersionTaskName,
                TaskVersionTaskDescription = entity.TaskVersionTaskDescription,
                TaskVersionTaskPoint = entity.TaskVersionTaskPoint,
                TaskVersionTaskDeadline = entity.TaskVersionTaskDeadline.FormatTime(),
                TaskVersionTaskStatus = entity.TaskVersionTaskStatus,
                TaskVersionTaskCompletedPercent = entity.TaskVersionTaskCompletedPercent,
            };
        }
    }
}
