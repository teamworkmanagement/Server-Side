using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.DTOs.TaskVersion;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TaskVersionRepository : ITaskVersionRepository
    {
        private readonly KhoaLuanContext _dbContext;

        public TaskVersionRepository(KhoaLuanContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> DeleteById(string taskVerId)
        {
            var entity = await _dbContext.TaskVersion.FindAsync(taskVerId);
            if (entity == null)
                return false;
            entity.TaskVersionTaskIsDeleted = true;
            _dbContext.TaskVersion.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<TaskVersionResponse>> GetAllByTaskId(string taskId)
        {
            var query = from tv in _dbContext.TaskVersion
                        where tv.TaskVersionTaskId == taskId
                        select tv;

            return await query.Select(x => new TaskVersionResponse
            {
                TaskVersionId = x.TaskVersionId,
                TaskVersionTaskId = x.TaskVersionTaskId,
                TaskVersionUpdatedAt = x.TaskVersionUpdatedAt,
                TaskVersionTaskName = x.TaskVersionTaskName,
                TaskVersionTaskDescription = x.TaskVersionTaskDescription,
                TaskVersionTaskPoint = x.TaskVersionTaskPoint,
                TaskVersionTaskDeadline = x.TaskVersionTaskDeadline,
                TaskVersionTaskStatus = x.TaskVersionTaskStatus,
                TaskVersionTaskCompletedPercent = x.TaskVersionTaskCompletedPercent,
                TaskVersionTaskIsDeleted = x.TaskVersionTaskIsDeleted,
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
                TaskVersionUpdatedAt = entity.TaskVersionUpdatedAt,
                TaskVersionTaskName = entity.TaskVersionTaskName,
                TaskVersionTaskDescription = entity.TaskVersionTaskDescription,
                TaskVersionTaskPoint = entity.TaskVersionTaskPoint,
                TaskVersionTaskDeadline = entity.TaskVersionTaskDeadline,
                TaskVersionTaskStatus = entity.TaskVersionTaskStatus,
                TaskVersionTaskCompletedPercent = entity.TaskVersionTaskCompletedPercent,
                TaskVersionTaskIsDeleted = entity.TaskVersionTaskIsDeleted,
            };
        }
    }
}
