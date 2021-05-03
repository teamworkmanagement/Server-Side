using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Task;

namespace TeamApp.WebApi.Hubs.Kanban
{
    public interface IHubKanbanClient
    {
        Task EditTask(TaskUpdateRequest message);
        Task AddTask(TaskRequest taskRequest);
        Task RemoveTask(TaskRequest taskRequest);
    }
}
