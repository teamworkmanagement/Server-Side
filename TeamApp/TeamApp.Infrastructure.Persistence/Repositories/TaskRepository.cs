using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Models.Task;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        public Task<string> AddTask(TaskRequest taskReq)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTask(string taskId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TaskResponse>> GetAllByTeamId(string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TaskResponse>> GetAllByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TaskResponse>> GetAllByUserTeamId(string userId, string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResponse<TaskResponse>> GetPaging(RequestParameter parameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateTask(string taskId, TaskRequest taskReq)
        {
            throw new NotImplementedException();
        }
    }
}
