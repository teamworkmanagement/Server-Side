using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Task;
using TeamApp.Application.Filters;
using TeamApp.Application.Wrappers;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        Task<string> AddTask(TaskRequest taskReq);
        Task<bool> UpdateTask(TaskUpdateRequest taskReq);
        Task<bool> DeleteTask(string taskId);
        Task<TaskResponse> GetById(string taskId);
        Task<bool> DragTask(DragTaskModel dragTaskModel);
        Task<TaskResponse> GetTaskByBoard(string userId, TaskGetRequest taskGetRequest);
    }
}
