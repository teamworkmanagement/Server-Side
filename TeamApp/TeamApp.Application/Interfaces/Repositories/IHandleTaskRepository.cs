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
        Task<bool> ReAssignTask(ReAssignModel reAssignModel);
    }
}
