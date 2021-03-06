using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.TaskVersion;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface ITaskVersionRepository
    {
        Task<string> AddTaskVersion(TaskVersionRequest taskVersionRequest);
        Task<TaskVersionResponse> GetById(string taskVerId);
        Task<List<TaskVersionResponse>> GetAllByTaskId(string taskId);
    }
}
