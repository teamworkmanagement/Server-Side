using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Statistics;
using TeamApp.Application.Exceptions;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IConfiguration _config;
        public StatisticsRepository(TeamAppContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        public async Task<int> GetUserTaskDoneCount(StatisticsRequest statisticsRequest)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            var startString = statisticsRequest.StartDate.Value.ToString("yyyy-MM-dd");
            var endString = statisticsRequest.EndDate.Value.ToString("yyyy-MM-dd");

            var query = "select count(taskdone.task_id) " +

                        "from(select task.task_id, task.task_kanbanlist_id " +
                        "from task " +
                        $"where date(task.task_done_date) >= '{startString}' and date(task.task_done_date) < '{endString}') taskdone " +

                        "join kanban_list on kanban_list.kanban_list_id = taskdone.task_kanbanlist_id " +
                        "join kanban_board on kanban_board.kanban_board_id = kanban_list.kanban_list_belonged_id " +
                        $"where kanban_board.kanban_board_userid = '{statisticsRequest.UserId}' ";

            //var listOuput = await Helpers.RawQuery.RawSqlQuery(_dbContext, query, (x) => int.Parse((string)x[0]));


            using (var connection = new MySqlConnection(connectionString))
            {
                var counts = await connection.QueryAsync<int>(query);
                var outPut = counts.ToList();
                return outPut[0];
            }
        }

        public async Task<int> GetBoardTaskDoneCount(StatisticsRequest statisticsRequest)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");
            var startString = statisticsRequest.StartDate.Value.ToString("yyyy-MM-dd");
            var endString = statisticsRequest.EndDate.Value.ToString("yyyy-MM-dd");

            var query = "select count(taskdone.task_id) " +

                        "from(select task.task_id, task.task_kanbanlist_id " +
                        "from task " +
                        $"where date(task.task_done_date) >= '{startString}' and date(task.task_done_date) < '{endString}') taskdone " +

                        "where taskdone.task_kanbanlist_id in " +
                        "(select kanban_list.kanban_list_id " +
                        "from kanban_list, kanban_board " +
                        $"where kanban_list.kanban_list_belonged_id = kanban_board.kanban_board_id and kanban_board.kanban_board_id = '{statisticsRequest.BoardId}') ";

            using (var connection = new MySqlConnection(connectionString))
            {
                var counts = await connection.QueryAsync<int>(query);
                var outPut = counts.ToList();
                return outPut[0];
            }
        }

        public async Task<List<UsersTaskDoneAndPointResponse>> GetUsersTaskDoneAndPointResponse(StatisticsRequest statisticsRequest)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            var startString = statisticsRequest.StartDate.Value.ToString("yyyy-MM-dd");
            var endString = statisticsRequest.EndDate.Value.ToString("yyyy-MM-dd");

            var query = "select user.user_fullname as UserFullName, count(*) as TaskDoneCount, sum(taskdoneboard.task_point) as Point " +

                        "from( " +
                        "select taskdone.task_id, taskdone.task_point " +
                        "from(select task.task_id, task.task_kanbanlist_id, task.task_point " +
                        "from task " +
                        $"where date(task.task_done_date) >= '{startString}' and  date(task.task_done_date) < '{endString}') taskdone " +

                        "where taskdone.task_kanbanlist_id in " +
                        "(select kanban_list.kanban_list_id " +
                        "from kanban_list " +
                        $"where kanban_list.kanban_list_belonged_id = '{statisticsRequest.BoardId}')) taskdoneboard " +

                        "join handle_task on handle_task.handle_task_task_id = taskdoneboard.task_id " +
                        "join user on handle_task.handle_task_user_id = user.user_id " +
                        "group by handle_task.handle_task_user_id";


            using (var connection = new MySqlConnection(connectionString))
            {
                var counts = await connection.QueryAsync<UsersTaskDoneAndPointResponse>(query);
                var outPut = counts.ToList();
                var random = new Random();
                foreach (var e in outPut)
                {
                    e.ColorCode = string.Format("#{0:X6}", random.Next(0x1000000));
                }
                return outPut;
            }
        }
        public async Task<List<int>> GetUserTaskDone(UserTaskDoneRequest userTaskDoneRequest)
        {
            var user = await _dbContext.User.FindAsync(userTaskDoneRequest.UserId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            DateTime startDate, endDate;

            List<StartEndDate> startEndDates = new List<StartEndDate>(); ;

            switch (userTaskDoneRequest.Filter)
            {
                case "week":
                    startEndDates = new List<StartEndDate>();
                    var now = DateTime.UtcNow.Date;
                    
                    for (int i = 0; i < 7; i++)
                    {
                        startEndDates.Add(new StartEndDate
                        {
                            StartDate = now.AddDays(-i),
                            EndDate = now.AddDays(-i + 1),
                        });
                    }

                    break;
                case "month":
                    startEndDates = new List<StartEndDate>();
                    now = DateTime.UtcNow.Date;
                    for (int i = 0; i < 30; i++)
                    {
                        startEndDates.Add(new StartEndDate
                        {
                            StartDate = now.AddDays(-i),
                            EndDate = now.AddDays(-i + 1),
                        });
                    }
                    break;
                case "year":
                    startEndDates = new List<StartEndDate>();
                    now = DateTime.UtcNow.Date;
                    for (int i = 0; i < 12; i++)
                    {
                        var newNow = now.AddMonths(-i);
                        startDate = new DateTime(newNow.Year, newNow.Month, 1);
                        endDate = startDate.AddMonths(1).AddDays(-1);

                        startEndDates.Add(new StartEndDate
                        {
                            StartDate = startDate,
                            EndDate = endDate,
                        });
                    }
                    break;
                default:
                    throw new ApiException("Filter error");
            }

            Dictionary<DateTime, int> keyValuePairs = new Dictionary<DateTime, int>();
            foreach (var e in startEndDates)
            {
                var count = await GetUserTaskDoneCount(new StatisticsRequest
                {
                    UserId = userTaskDoneRequest.UserId,
                    Filter = userTaskDoneRequest.Filter,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                });

                keyValuePairs[e.StartDate] = count;
            }

            var response = new List<int>();

            foreach (var kv in keyValuePairs)
            {
                response.Add(kv.Value);
            }

            response.Reverse();

            return response;
        }

        public async Task<List<int>> GetBoardTaskDone(BoardTaskDoneRequest boardTaskDoneRequest)
        {
            var board = await _dbContext.KanbanBoard.FindAsync(boardTaskDoneRequest.BoardId);
            if (board == null)
                throw new KeyNotFoundException("Board not found");

            DateTime startDate;
            DateTime endDate;

            List<StartEndDate> startEndDates = new List<StartEndDate>(); ;

            switch (boardTaskDoneRequest.Filter)
            {
                case "week":
                    startEndDates = new List<StartEndDate>();
                    var now = DateTime.UtcNow.Date;
                    Console.WriteLine("======================================================");
                    for (int i = 0; i < 7; i++)
                    {
                        startEndDates.Add(new StartEndDate
                        {
                            StartDate = now.AddDays(-i),
                            EndDate = now.AddDays(-i + 1),
                        });
                        //Console.WriteLine(now.AddDays(-i) + "-----------" + now.AddDays(-i + 1));
                    }

                    break;
                case "month":
                    startEndDates = new List<StartEndDate>();
                    now = DateTime.UtcNow.Date;
                    Console.WriteLine("=====================================================");
                    for (int i = 0; i < 30; i++)
                    {
                        startEndDates.Add(new StartEndDate
                        {
                            StartDate = now.AddDays(-i),
                            EndDate = now.AddDays(-i + 1),
                        });
                        //Console.WriteLine(now.AddDays(-i) + "-----------" + now.AddDays(-i + 1));
                    }
                    break;
                case "year":
                    startEndDates = new List<StartEndDate>();
                    now = DateTime.UtcNow.Date;
                    Console.WriteLine("=====================================================");
                    for (int i = 0; i < 12; i++)
                    {
                        var newNow = now.AddMonths(-i);
                        startDate = new DateTime(newNow.Year, newNow.Month, 1);
                        endDate = startDate.AddMonths(1).AddDays(-1);

                        startEndDates.Add(new StartEndDate
                        {
                            StartDate = startDate,
                            EndDate = endDate,
                        });
                        //Console.WriteLine(startDate + "------------------" + endDate);
                    }
                    break;
                default:
                    throw new ApiException("Filter error");
            }

            Dictionary<DateTime, int> keyValuePairs = new Dictionary<DateTime, int>();
            foreach (var e in startEndDates)
            {
                var count = await GetBoardTaskDoneCount(new StatisticsRequest
                {
                    Filter = boardTaskDoneRequest.Filter,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    BoardId = boardTaskDoneRequest.BoardId,
                });

                keyValuePairs[e.StartDate] = count;
                Console.WriteLine(e.StartDate + "================" + e.EndDate);
            }

            var response = new List<int>();

            foreach (var kv in keyValuePairs)
            {
                response.Add(kv.Value);
            }

            response.Reverse();
            return response;
        }

        async Task<int> GetUserTaskDoneInBoardsCount(StatisticsRequest statisticsRequest)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            var startString = statisticsRequest.StartDate.Value.ToString("yyyy-MM-dd");
            var endString = statisticsRequest.EndDate.Value.ToString("yyyy-MM-dd");

            var query = "select count(*) as taskdones " +
                        "from(select task.task_id " +

                        "from task " +

                        $"where date(task.task_done_date) >= '{startString}' and date(task.task_done_date) < '{endString}' ) taskdone " +
                        "join handle_task on handle_task.handle_task_task_id = taskdone.task_id " +
                        $"where handle_task.handle_task_user_id = '${statisticsRequest.UserId}'";


            using (var connection = new MySqlConnection(connectionString))
            {
                var counts = await connection.QueryAsync<int>(query);
                var outPut = counts.ToList();
                return outPut[0];
            }
        }

        public async Task<List<UsersTaskDoneAndPointResponse>> GetUsersTaskDoneAndPoint(UsersTaskDoneAndPointRequest request)
        {
            var board = await _dbContext.KanbanBoard.FindAsync(request.BoardId);
            if (board == null)
                throw new KeyNotFoundException("board not found");

            DateTime startDate;
            DateTime endDate;

            var now = DateTime.UtcNow.Date;
            startDate = now.AddDays(-7);
            endDate = now.AddDays(1);

            var response = await GetUsersTaskDoneAndPointResponse(new StatisticsRequest
            {
                BoardId = request.BoardId,
                StartDate = startDate,
                EndDate = endDate,
            });

            return response;
        }

        public async Task<List<int>> GetUserTaskDoneInBoards(UserTaskDoneInBoardsRequest userTaskDoneInBoardsRequest)
        {
            var user = await _dbContext.User.FindAsync(userTaskDoneInBoardsRequest.UserId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            DateTime startDate;
            DateTime endDate;

            List<StartEndDate> startEndDates = new List<StartEndDate>(); ;

            switch (userTaskDoneInBoardsRequest.Filter)
            {
                case "week":
                    startEndDates = new List<StartEndDate>();
                    var now = DateTime.UtcNow.Date;
                    Console.WriteLine("======================================================");
                    for (int i = 0; i < 7; i++)
                    {
                        startEndDates.Add(new StartEndDate
                        {
                            StartDate = now.AddDays(-i),
                            EndDate = now.AddDays(-i + 1),
                        });
                        //Console.WriteLine(now.AddDays(-i) + "-----------" + now.AddDays(-i + 1));
                    }
                    break;
                case "month":
                    startEndDates = new List<StartEndDate>();
                    now = DateTime.UtcNow.Date;
                    Console.WriteLine("=====================================================");
                    for (int i = 0; i < 30; i++)
                    {
                        startEndDates.Add(new StartEndDate
                        {
                            StartDate = now.AddDays(-i),
                            EndDate = now.AddDays(-i + 1),
                        });
                        //Console.WriteLine(now.AddDays(-i) + "-----------" + now.AddDays(-i + 1));
                    }
                    break;
                case "year":
                    startEndDates = new List<StartEndDate>();
                    now = DateTime.UtcNow.Date;
                    Console.WriteLine("=====================================================");
                    for (int i = 0; i < 12; i++)
                    {
                        var newNow = now.AddMonths(-i);
                        startDate = new DateTime(newNow.Year, newNow.Month, 1);
                        endDate = startDate.AddMonths(1).AddDays(-1);

                        startEndDates.Add(new StartEndDate
                        {
                            StartDate = startDate,
                            EndDate = endDate,
                        });
                        //Console.WriteLine(startDate + "------------------" + endDate);
                    }
                    break;
                default:
                    throw new ApiException("Filter error");
            }

            Dictionary<DateTime, int> keyValuePairs = new Dictionary<DateTime, int>();
            foreach (var e in startEndDates)
            {
                var count = await GetUserTaskDoneInBoardsCount(new StatisticsRequest
                {
                    Filter = userTaskDoneInBoardsRequest.Filter,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                });

                keyValuePairs[e.StartDate] = count;
            }

            var response = new List<int>();

            foreach (var kv in keyValuePairs)
            {
                response.Add(kv.Value);
            }

            response.Reverse();

            return response;
        }

        public async Task<byte[]> ExportPersonalAndTeamsTask(ExportPersonalAndTeamsTaskRequest exportPersonal)
        {
            var package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Công việc cá nhân và nhóm");
            // create title
            workSheet.Cells["A1:D1"].Merge = true;
            workSheet.Cells["A1"].Value = "Công việc cá nhân và nhóm";
            workSheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["A1"].Style.Font.Bold = true;
            // fill header
            List<string> listHeader = new List<string>()
            {
                "A2","B2","C2","D2",
            };
            listHeader.ForEach(c =>
            {
                workSheet.Cells[c].Style.Font.Bold = true;
                workSheet.Cells[c].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[c].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[c].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[c].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            });

            workSheet.Cells[listHeader[0]].Value = "STT";
            workSheet.Cells[listHeader[1]].Value = "Thời gian";
            workSheet.Cells[listHeader[2]].Value = "Công việc cá nhân hoàn thành";
            workSheet.Cells[listHeader[3]].Value = "Công việc nhóm hoàn thành";

            //fill data
            for (int i = 0; i < exportPersonal.UserStatis.Count; i++)
            {
                DateTime dt = new DateTime();
                if (exportPersonal.UserStatis.Count == 7 || exportPersonal.UserStatis.Count == 30)
                    dt = DateTime.UtcNow.AddDays(-(exportPersonal.UserStatis.Count - i - 1));

                if (exportPersonal.UserStatis.Count == 12)
                {
                    dt = DateTime.UtcNow.AddMonths(-(exportPersonal.UserStatis.Count - i - 1));
                }
                workSheet.Cells[i + 3, 1].Value = (i + 1).ToString();
                workSheet.Cells[i + 3, 2].Value = exportPersonal.UserStatis.Count != 12 ? dt.ToString("dd/MM/yyyy", new CultureInfo("vi-VN")) : dt.ToString("MM/yyyy", new CultureInfo("vi-VN"));
                workSheet.Cells[i + 3, 3].Value = exportPersonal.UserStatis[i];
                workSheet.Cells[i + 3, 4].Value = exportPersonal.TeamStatis[i];
            }
            // format column width
            for (int i = 1; i < 5; i++)
            {
                switch (i)
                {
                    case 2:
                        workSheet.Column(i).Width = 50;
                        break;
                    case 3:
                        workSheet.Column(i).Width = 50;
                        break;
                    case 4:
                        workSheet.Column(i).Width = 50;
                        break;
                    default:
                        workSheet.Column(i).Width = 20;
                        break;
                }
            }

            // format cell border
            for (int i = 0; i < exportPersonal.UserStatis.Count; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    workSheet.Cells[i + 3, j].Style.Font.Size = 10;
                    workSheet.Cells[i + 3, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 3, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 3, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 3, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
            }
            return await package.GetAsByteArrayAsync();
        }

        public  async Task<byte[]> ExportBoardDoneTask(BoardDoneTaskExportRequest exportRequest)
        {
            var package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Công việc nhóm");
            // create title
            workSheet.Cells["A1:D1"].Merge = true;
            workSheet.Cells["A1"].Value = "Công việc nhóm";
            workSheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["A1"].Style.Font.Bold = true;
            // fill header
            List<string> listHeader = new List<string>()
            {
                "A2","B2","C2",
            };
            listHeader.ForEach(c =>
            {
                workSheet.Cells[c].Style.Font.Bold = true;
                workSheet.Cells[c].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[c].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[c].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[c].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            });

            workSheet.Cells[listHeader[0]].Value = "STT";
            workSheet.Cells[listHeader[1]].Value = "Thời gian";
            workSheet.Cells[listHeader[2]].Value = "Công việc hoàn thành";

            //fill data
            for (int i = 0; i < exportRequest.BoardTaskDone.Count; i++)
            {
                DateTime dt = new DateTime();
                if (exportRequest.BoardTaskDone.Count == 7 || exportRequest.BoardTaskDone.Count == 30)
                    dt = DateTime.UtcNow.AddDays(-(exportRequest.BoardTaskDone.Count - i - 1));

                if (exportRequest.BoardTaskDone.Count == 12)
                {
                    dt = DateTime.UtcNow.AddMonths(-(exportRequest.BoardTaskDone.Count - i - 1));
                }
                workSheet.Cells[i + 3, 1].Value = (i + 1).ToString();
                workSheet.Cells[i + 3, 2].Value = exportRequest.BoardTaskDone.Count != 12 ? dt.ToString("dd/MM/yyyy", new CultureInfo("vi-VN")) : dt.ToString("MM/yyyy", new CultureInfo("vi-VN"));
                workSheet.Cells[i + 3, 3].Value = exportRequest.BoardTaskDone[i];
            }
            // format column width
            for (int i = 1; i < 4; i++)
            {
                switch (i)
                {
                    case 1:
                        workSheet.Column(i).Width = 50;
                        break;
                    case 2:
                        workSheet.Column(i).Width = 50;
                        break;
                    case 3:
                        workSheet.Column(i).Width = 50;
                        break;
                    default:
                        workSheet.Column(i).Width = 20;
                        break;
                }
            }

            // format cell border
            for (int i = 0; i < exportRequest.BoardTaskDone.Count; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    workSheet.Cells[i + 3, j].Style.Font.Size = 10;
                    workSheet.Cells[i + 3, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 3, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 3, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 3, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
            }
            return await package.GetAsByteArrayAsync();
        }

        public async Task<byte[]> ExportUserBoardDonePointAndTask(BoardPointAndDoneRequest pointAndDoneRequest)
        {
            var package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Công việc nhóm và tổng điểm");
            // create title
            workSheet.Cells["A1:D1"].Merge = true;
            workSheet.Cells["A1"].Value = "Công việc nhóm và tổng điểm";
            workSheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["A1"].Style.Font.Bold = true;
            // fill header
            List<string> listHeader = new List<string>()
            {
                "A2","B2","C2","D2",
            };
            listHeader.ForEach(c =>
            {
                workSheet.Cells[c].Style.Font.Bold = true;
                workSheet.Cells[c].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[c].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[c].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[c].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            });

            workSheet.Cells[listHeader[0]].Value = "STT";
            workSheet.Cells[listHeader[1]].Value = "Thành viên";
            workSheet.Cells[listHeader[2]].Value = "Tổng điểm";
            workSheet.Cells[listHeader[3]].Value = "Tổng công việc";

            //fill data
            for (int i = 0; i < pointAndDoneRequest.RequestModels.Count; i++)
            {
                workSheet.Cells[i + 3, 1].Value = (i + 1).ToString();
                workSheet.Cells[i + 3, 2].Value = pointAndDoneRequest.RequestModels[0].UserFullName;
                workSheet.Cells[i + 3, 3].Value = pointAndDoneRequest.RequestModels[0].Point;
                workSheet.Cells[i + 3, 4].Value = pointAndDoneRequest.RequestModels[0].TaskDoneCount;
            }
            // format column width
            for (int i = 1; i < 5; i++)
            {
                switch (i)
                {
                    case 2:
                        workSheet.Column(i).Width = 50;
                        break;
                    case 3:
                        workSheet.Column(i).Width = 50;
                        break;
                    case 4:
                        workSheet.Column(i).Width = 50;
                        break;
                    default:
                        workSheet.Column(i).Width = 20;
                        break;
                }
            }

            // format cell border
            for (int i = 0; i < pointAndDoneRequest.RequestModels.Count; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    workSheet.Cells[i + 3, j].Style.Font.Size = 10;
                    workSheet.Cells[i + 3, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 3, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 3, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 3, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
            }
            return await package.GetAsByteArrayAsync();
        }
    }
}
