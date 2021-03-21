using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Domain.Models.HanldeTask;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IHandleTaskRepository
    {
        Task<List<HandleTaskResponse>> GetAllByTaskId(string taskId);
        Task<List<HandleTaskResponse>> GetAllByUserId(string userId);
        Task<string> AddHandleTask(HandleTaskRequest handleTaskReq);
        Task<bool> UpdateHandleTask(string handleTaskId, HandleTaskRequest handleTaskReq);
        Task<bool> DeleteHandleTask(string handleTaskId);
    }
}
