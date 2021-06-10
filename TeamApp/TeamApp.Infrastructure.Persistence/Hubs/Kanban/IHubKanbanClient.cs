using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanBoard;
using TeamApp.Application.DTOs.KanbanList;
using TeamApp.Application.DTOs.Task;

namespace TeamApp.Infrastructure.Persistence.Hubs.Kanban
{
    public interface IHubKanbanClient
    {
        Task AddNewTask(TaskUIKanban newTask);
        Task AddNewList(KanbanListUIResponse newList);
        Task RemoveTask(TaskRemoveModel taskRemoveModel);
        Task RemoveList(KanbanListUIResponse newTask);
        Task MoveTask(DragTaskModel newTask);
        Task MoveList(SwapListModel swapListModel);
        Task UpdateTask(object newTask);
        Task UpdateList(KanbanListRequest newTask);
        Task ReAssignUser(object reAssignObject);
    }
}
