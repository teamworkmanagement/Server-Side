using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.HandleTask;
using TeamApp.Application.DTOs.Task;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IHandleTaskRepository
    {
        Task<List<HandleTaskResponse>> GetAllByTaskId(string taskId);
        Task<List<HandleTaskResponse>> GetAllByUserId(string userId);
        Task<HandleTaskResponse> AddHandleTask(HandleTaskRequest handleTaskReq);
        Task<bool> UpdateHandleTask(string handleTaskId, HandleTaskRequest handleTaskReq);
        Task<bool> DeleteHandleTask(string handleTaskId);
        Task<bool> ReAssignTask(ReAssignModel reAssignModel);
    }
}
