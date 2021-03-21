using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Domain.Models.TaskVersion;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface ITaskVersionRepository
    {
        Task<TaskVersionResponse> GetById(string taskVerId);
        Task<List<TaskVersionResponse>> GetAllByTaskId(string taskId);
        Task<bool> DeleteById(string taskVerId);
    }
}
