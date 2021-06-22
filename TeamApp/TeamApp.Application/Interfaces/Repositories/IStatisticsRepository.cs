using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Statistics;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IStatisticsRepository
    {
        Task<List<int>> GetUserTaskDone(UserTaskDoneRequest userTaskDoneRequest);
        Task<List<int>> GetUserTaskDoneInBoards(UserTaskDoneInBoardsRequest userTaskDoneInBoardsRequest);
        Task<List<int>> GetBoardTaskDone(BoardTaskDoneRequest boardTaskDoneRequest);
        Task<List<UsersTaskDoneAndPointResponse>> GetUsersTaskDoneAndPoint(UsersTaskDoneAndPointRequest usersTaskDoneAndPointRequest);


        Task<byte[]> ExportPersonalAndTeamsTask(ExportPersonalAndTeamsTaskRequest exportPersonal);
        Task<byte[]> ExportBoardDoneTask(BoardDoneTaskExportRequest exportPersonal);
        Task<byte[]> ExportUserBoardDonePointAndTask(BoardPointAndDoneRequest exportPersonal);
    }
}
