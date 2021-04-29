using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanBoard;
using TeamApp.Application.DTOs.KanbanList;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using TeamApp.Application.DTOs.Task;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.Utils;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class KanbanBoardRepository : IKanbanBoardRepository
    {
        private readonly TeamAppContext _dbContext;
        public KanbanBoardRepository(TeamAppContext teamAppContext)
        {
            _dbContext = teamAppContext;
        }

        public async Task<KanbanBoardResponse> AddKanbanBoard(KanbanBoardRequest kanbanBoardRequest)
        {
            var entity = new KanbanBoard
            {
                KanbanBoardId = Guid.NewGuid().ToString(),
                KanbanBoardIsOfTeam = kanbanBoardRequest.KanbanBoardIsOfTeam,
                KanbanBoardBelongedId = kanbanBoardRequest.KanbanBoardBelongedId,
            };

            await _dbContext.KanbanBoard.AddAsync(entity);
            var check = await _dbContext.SaveChangesAsync() > 0;

            if (check)
            {
                return new KanbanBoardResponse
                {
                    KanbanBoardId = entity.KanbanBoardId,
                    KanbanBoardIsOfTeam = entity.KanbanBoardIsOfTeam,
                    KanbanBoardBelongedId = entity.KanbanBoardBelongedId,
                };
            }
            return null;
        }

        public async Task<KanbanBoardUIResponse> GetKanbanBoardUI(string boardId)
        {
            var board = await _dbContext.KanbanBoard.FindAsync(boardId);
            if (board == null)
                return null;

            var taskListUI = new List<TaskUIKanban>();
            var outPut = new KanbanBoardUIResponse
            {
                KanbanBoardId = board.KanbanBoardId,
                KanbanBoardBelongedId = board.KanbanBoardBelongedId,
                KanbanBoardIsOfTeam = board.KanbanBoardIsOfTeam,
                KanbanListUI = new List<KanbanListUIResponse>(),
            };

            //danh sach kanbanlist cua 1 board
            var listKanban = from kl in _dbContext.KanbanList
                             where kl.KanbanListBoardBelongedId == boardId
                             orderby kl.KanbanListOrderInBoard
                             select kl;

            var listKb = await listKanban.ToListAsync();
            //duyet tat ca cac kanbanlist de lay ra danh sach task cua moi list
            foreach (var kl in listKb)
            {
                var tasklist = from t in _dbContext.Task
                               join h in _dbContext.HandleTask on t.TaskId equals h.HandleTaskTaskId
                               join u in _dbContext.User on h.HandleTaskUserId equals u.Id
                               where t.TaskBelongedId == kl.KanbanListId && t.TaskIsDeleted == false
                               select new { t, u.ImageUrl, u.Id };

                //tasks of 1 listkanban
                var taskLists = await tasklist.ToListAsync();

                taskListUI = new List<TaskUIKanban>();

                //cac task trong list kanban
                foreach (var x in taskLists)
                {
                    var files = await _dbContext.File.Where(f => f.FileBelongedId == x.t.TaskId).OrderByDescending(f => f.FileUploadTime).ToListAsync();

                    var image = string.Empty;


                    foreach (var f in files)
                    {
                        if (f.FileType == "png")
                        {
                            image = f.FileUrl;
                            break;
                        }
                    }

                    var taskUIKanban = new TaskUIKanban
                    {
                        OrderInList = x.t.TaskOrderInList,
                        KanbanListId = x.t.TaskBelongedId,
                        TaskId = x.t.TaskId,

                        TaskName = x.t.TaskName,
                        TaskDeadline = x.t.TaskDeadline.FormatTime(),
                        TaskStatus = x.t.TaskStatus,
                        TaskDescription = x.t.TaskDescription,

                        Image = string.IsNullOrEmpty(image) ? null : image,

                        CommentsCount = x.t.Comments.Count,
                        FilesCount = files.Count,

                        UserId = x.Id,
                        UserAvatar = x.ImageUrl,

                        TaskCompletedPercent = x.t.TaskCompletedPercent,
                    };

                    taskListUI.Add(taskUIKanban);
                }

                var kbListUi = new KanbanListUIResponse
                {
                    KanbanListId = kl.KanbanListId,
                    KanbanListTitle = kl.KanbanListTitle,
                    KanbanListBoardBelongedId = kl.KanbanListBoardBelongedId,
                    KanbanListOrderInBoard = kl.KanbanListOrderInBoard,
                    TaskUIKanban = taskListUI,
                };

                outPut.KanbanListUI.Add(kbListUi);
            }
            return outPut;
        }

        public Task<List<KanbanListUIResponse>> GetKanbanList(string boardId)
        {
            throw new NotImplementedException();
        }
    }
}
