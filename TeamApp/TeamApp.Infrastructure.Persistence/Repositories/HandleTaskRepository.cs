using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.HanldeTask;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class HandleTaskRepository : IHandleTaskRepository
    {
        public Task<string> AddHandleTask(HandleTaskRequest handleTaskReq)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteHandleTask(string handleTaskId)
        {
            throw new NotImplementedException();
        }

        public Task<List<HandleTaskResponse>> GetAllByTaskId(string taskId)
        {
            throw new NotImplementedException();
        }

        public Task<List<HandleTaskResponse>> GetAllByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateHandleTask(string handleTaskId, HandleTaskRequest handleTaskReq)
        {
            throw new NotImplementedException();
        }
    }
}
