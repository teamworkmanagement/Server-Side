using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.TaskVersion;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TaskVersionRepository : ITaskVersionRepository
    {
        public Task<bool> DeleteById(string taskVerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TaskVersionResponse>> GetAllByTaskId(string taskId)
        {
            throw new NotImplementedException();
        }

        public Task<TaskVersionResponse> GetById(string taskVerId)
        {
            throw new NotImplementedException();
        }
    }
}
