using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.DTOs.HandleTask;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class HandleTaskRepository : IHandleTaskRepository
    {
        private readonly TeamAppContext _dbContext;

        public HandleTaskRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> AddHandleTask(HandleTaskRequest handleTaskReq)
        {
            var entity = new HandleTask
            {
                HandleTaskId = Guid.NewGuid().ToString(),
                HandleTaskUserId = handleTaskReq.HandleTaskUserId,
                HandleTaskTaskId = handleTaskReq.HandleTaskTaskId,
                HandleTaskCreatedAt = handleTaskReq.HandleTaskCreatedAt,
                HandleTaskIsDeleted = handleTaskReq.HandleTaskIsDeleted,
            };

            await _dbContext.HandleTask.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.HandleTaskId;
        }

        public async Task<bool> DeleteHandleTask(string handleTaskId)
        {
            var entity = await _dbContext.HandleTask.FindAsync(handleTaskId);
            if (entity == null)
                return false;

            entity.HandleTaskIsDeleted = true;
            _dbContext.HandleTask.Update(entity);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<HandleTaskResponse>> GetAllByTaskId(string taskId)
        {
            var query = from ht in _dbContext.HandleTask
                        where ht.HandleTaskTaskId == taskId
                        select ht;

            var outPut = await query.Select(x => new HandleTaskResponse
            {
                HandleTaskId = x.HandleTaskId,
                HandleTaskUserId = x.HandleTaskUserId,
                HandleTaskTaskId = x.HandleTaskTaskId,
                HandleTaskCreatedAt = x.HandleTaskCreatedAt,
                HandleTaskIsDeleted = x.HandleTaskIsDeleted,
            }).ToListAsync();

            return outPut;
        }

        public async Task<List<HandleTaskResponse>> GetAllByUserId(string userId)
        {
            var query = from ht in _dbContext.HandleTask
                        where ht.HandleTaskUserId == userId
                        select ht;

            var outPut = await query.Select(x => new HandleTaskResponse
            {
                HandleTaskId = x.HandleTaskId,
                HandleTaskUserId = x.HandleTaskUserId,
                HandleTaskTaskId = x.HandleTaskTaskId,
                HandleTaskCreatedAt = x.HandleTaskCreatedAt,
                HandleTaskIsDeleted = x.HandleTaskIsDeleted,
            }).ToListAsync();

            return outPut;
        }

        public async Task<bool> UpdateHandleTask(string handleTaskId, HandleTaskRequest handleTaskReq)
        {
            var entity = await _dbContext.HandleTask.FindAsync(handleTaskId);
            if (entity == null)
                return false;

            entity.HandleTaskUserId = handleTaskReq.HandleTaskUserId;
            entity.HandleTaskTaskId = handleTaskReq.HandleTaskTaskId;
            entity.HandleTaskCreatedAt = handleTaskReq.HandleTaskCreatedAt;
            entity.HandleTaskIsDeleted = handleTaskReq.HandleTaskIsDeleted;

            _dbContext.HandleTask.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
