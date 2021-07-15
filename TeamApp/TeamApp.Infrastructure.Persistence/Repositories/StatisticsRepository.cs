using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
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
                var count = await GetBoardTaskDoneCount(new StatisticsRequest
                {
                    Filter = boardTaskDoneRequest.Filter,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    BoardId = boardTaskDoneRequest.BoardId,
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
                        $"where handle_task.handle_task_user_id = '{statisticsRequest.UserId}'";


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
                var count = await GetUserTaskDoneInBoardsCount(new StatisticsRequest
                {
                    Filter = userTaskDoneInBoardsRequest.Filter,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    UserId = userTaskDoneInBoardsRequest.UserId,
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
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                exportPersonal.Image.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            var userStatis = JsonConvert.DeserializeObject<List<int>>(exportPersonal.UserStatis);
            var teamStatis = JsonConvert.DeserializeObject<List<int>>(exportPersonal.TeamStatis);

            var package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Công việc cá nhân và nhóm");

            var type = string.Empty;
            switch (teamStatis.Count)
            {
                case 7:
                    type = "tuần";
                    break;
                case 12:
                    type = "năm";
                    break;
                default:
                    type = "tháng";
                    break;
            }
            workSheet.Cells["A1:F50"].Style.Font.Size = 13;
            workSheet.Cells["A1:F50"].Style.Font.Name = "Times New Roman";
            // create title
            workSheet.Cells["A1:D1"].Merge = true;
            workSheet.Cells["A1"].Value = DateTime.UtcNow.AddHours(7).ToString("dddd, dd MMMM yyyy HH:mm:ss", new CultureInfo("vi-VN"));
            workSheet.Cells["A2:D2"].Merge = true;
            workSheet.Cells["A2"].Value = "Thống kê công việc cá nhân và nhóm trong " + type;
            workSheet.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["A2"].Style.Font.Bold = true;
            // fill header
            List<string> listHeader = new List<string>()
            {
                "A3","B3","C3","D3",
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
            for (int i = 0; i < userStatis.Count; i++)
            {
                DateTime dt = new DateTime();
                if (userStatis.Count == 7 || userStatis.Count == 30)
                    dt = DateTime.UtcNow.AddDays(-(userStatis.Count - i - 1));

                if (userStatis.Count == 12)
                {
                    dt = DateTime.UtcNow.AddMonths(-(userStatis.Count - i - 1));
                }
                workSheet.Cells[i + 4, 1].Value = (i + 1).ToString();
                workSheet.Cells[i + 4, 2].Value = userStatis.Count != 12 ? dt.ToString("dd/MM/yyyy", new CultureInfo("vi-VN")) : dt.ToString("MM/yyyy", new CultureInfo("vi-VN"));
                workSheet.Cells[i + 4, 3].Value = userStatis[i];
                workSheet.Cells[i + 4, 4].Value = teamStatis[i];
            }
            // format column width
            for (int i = 1; i < 5; i++)
            {
                switch (i)
                {
                    case 2:
                        workSheet.Column(i).Width = 55;
                        break;
                    case 3:
                        workSheet.Column(i).Width = 55;
                        break;
                    case 4:
                        workSheet.Column(i).Width = 55;
                        break;
                    default:
                        workSheet.Column(i).Width = 20;
                        break;
                }
            }

            // format cell border
            for (int i = 0; i < userStatis.Count; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    workSheet.Cells[i + 4, j].Style.Font.Size = 13;
                    workSheet.Cells[i + 4, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 4, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 4, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 4, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
            }

            using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
            {
                Image image = Image.FromStream(ms, true);

                ExcelPicture pic = workSheet.Drawings.AddPicture("report_img", image);

                pic.From.Column = 1;
                pic.From.Row = userStatis.Count + 4;

                //8+19 row, 8+14 column
                workSheet.Cells[userStatis.Count + 4, 1, userStatis.Count + 4 + 19, 4].Merge = true;
            }

            await package.SaveAsync();

            return await package.GetAsByteArrayAsync();
        }

        public async Task<byte[]> ExportBoardDoneTask(BoardDoneTaskExportRequest exportRequest)
        {
            var boardTaskDone = JsonConvert.DeserializeObject<List<int>>(exportRequest.BoardTaskDone);
            var package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Công việc nhóm");

            workSheet.Cells["A1:F50"].Style.Font.Size = 13;
            workSheet.Cells["A1:F50"].Style.Font.Name = "Times New Roman";

            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                exportRequest.Image.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            var type = string.Empty;
            switch (boardTaskDone.Count)
            {
                case 7:
                    type = "tuần";
                    break;
                case 12:
                    type = "năm";
                    break;
                default:
                    type = "tháng";
                    break;
            }

            // create title
            workSheet.Cells["A1:C1"].Merge = true;
            workSheet.Cells["A2:C2"].Merge = true;

            workSheet.Cells["A1"].Value = DateTime.UtcNow.AddHours(7).ToString("dddd, dd MMMM yyyy HH:mm:ss", new CultureInfo("vi-VN"));
            workSheet.Cells["A2"].Value = "Thống kê công việc bảng " + exportRequest.BoardName + " trong " + type;
            workSheet.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["A2"].Style.Font.Bold = true;
            // fill header
            List<string> listHeader = new List<string>()
            {
                "A3","B3","C3",
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
            for (int i = 0; i < boardTaskDone.Count; i++)
            {
                DateTime dt = new DateTime();
                if (boardTaskDone.Count == 7 || boardTaskDone.Count == 30)
                    dt = DateTime.UtcNow.AddDays(-(boardTaskDone.Count - i - 1));

                if (boardTaskDone.Count == 12)
                {
                    dt = DateTime.UtcNow.AddMonths(-(boardTaskDone.Count - i - 1));
                }
                workSheet.Cells[i + 4, 1].Value = (i + 1).ToString();
                workSheet.Cells[i + 4, 2].Value = boardTaskDone.Count != 12 ? dt.ToString("dd/MM/yyyy", new CultureInfo("vi-VN")) : dt.ToString("MM/yyyy", new CultureInfo("vi-VN"));
                workSheet.Cells[i + 4, 3].Value = boardTaskDone[i];
            }
            // format column width
            for (int i = 1; i < 4; i++)
            {
                workSheet.Column(i).Width = i switch
                {
                    1 => 15,
                    2 => 75,
                    3 => 75,
                    _ => 15,
                };
            }

            // format cell border
            for (int i = 0; i < boardTaskDone.Count; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    workSheet.Cells[i + 4, j].Style.Font.Size = 13;
                    workSheet.Cells[i + 4, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 4, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 4, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 4, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
            }

            using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
            {
                Image image = Image.FromStream(ms, true);

                ExcelPicture pic = workSheet.Drawings.AddPicture("report_img", image);

                pic.From.Column = 1;
                pic.From.Row = boardTaskDone.Count + 4;

                //8+19 row, 8+14 column
                workSheet.Cells[boardTaskDone.Count + 4, 1, boardTaskDone.Count + 4 + 19, 3].Merge = true;
            }

            await package.SaveAsync();

            return await package.GetAsByteArrayAsync();
        }

        public async Task<byte[]> ExportUserBoardDonePointAndTask(BoardPointAndDoneRequest pointAndDoneRequest)
        {
            var requestModels = JsonConvert.DeserializeObject<List<UsersTaskDoneAndPointResponse>>(pointAndDoneRequest.RequestModels);

            var package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Công việc nhóm và tổng điểm");

            workSheet.Cells["A1:F50"].Style.Font.Size = 13;
            workSheet.Cells["A1:F50"].Style.Font.Name = "Times New Roman";

            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                pointAndDoneRequest.Image.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            // create title
            workSheet.Cells["A1:D1"].Merge = true;
            workSheet.Cells["A2:D2"].Merge = true;
            workSheet.Cells["A1"].Value = DateTime.UtcNow.AddHours(7).ToString("dddd, dd MMMM yyyy HH:mm:ss", new CultureInfo("vi-VN"));
            workSheet.Cells["A2"].Value = "Thống kê công việc và tổng điểm trong bảng " + pointAndDoneRequest.BoardName;
            workSheet.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["A2"].Style.Font.Bold = true;
            // fill header
            List<string> listHeader = new List<string>()
            {
                "A3","B3","C3","D3",
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
            for (int i = 0; i < requestModels.Count; i++)
            {
                workSheet.Cells[i + 4, 1].Value = (i + 1).ToString();
                workSheet.Cells[i + 4, 2].Value = requestModels[i].UserFullName;
                workSheet.Cells[i + 4, 3].Value = requestModels[i].Point;
                workSheet.Cells[i + 4, 4].Value = requestModels[i].TaskDoneCount;
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
            for (int i = 0; i < requestModels.Count; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    workSheet.Cells[i + 4, j].Style.Font.Size = 13;
                    workSheet.Cells[i + 4, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 4, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 4, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 4, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
            }

            using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
            {
                Image image = Image.FromStream(ms, true);

                ExcelPicture pic = workSheet.Drawings.AddPicture("report_img", image);

                pic.From.Column = 1;
                pic.From.Row = requestModels.Count + 4;

                //8+19 row, 8+14 column
                workSheet.Cells[requestModels.Count + 4, 1, requestModels.Count + 4 + 19, 4].Merge = true;
            }

            await package.SaveAsync();

            return await package.GetAsByteArrayAsync();
        }

        async Task<int> ReportCount(TaskStatRequest taskStatRequest)
        {
            var count = 0;
            if (taskStatRequest.OwnerType == "personal")
            {
                if (taskStatRequest.StatusType == "todo")
                {
                    count = await (from t in _dbContext.Task.AsNoTracking()
                                   join kl in _dbContext.KanbanList.AsNoTracking() on t.TaskBelongedId equals kl.KanbanListId
                                   join b in _dbContext.KanbanBoard.AsNoTracking() on kl.KanbanListBoardBelongedId equals b.KanbanBoardId
                                   where b.KanbanBoardUserId == taskStatRequest.UserId && t.TaskIsDeleted != true
                                   && kl.KanbanListIsDeleted != true && t.TaskStatus == "todo"
                                   orderby t.TaskCreatedAt descending
                                   select t.TaskId).CountAsync();
                }
                else
                {
                    count = await (from t in _dbContext.Task.AsNoTracking()
                                   join kl in _dbContext.KanbanList.AsNoTracking() on t.TaskBelongedId equals kl.KanbanListId
                                   join b in _dbContext.KanbanBoard.AsNoTracking() on kl.KanbanListBoardBelongedId equals b.KanbanBoardId
                                   where b.KanbanBoardUserId == taskStatRequest.UserId && t.TaskIsDeleted != true
                                   && kl.KanbanListIsDeleted != true && t.TaskDeadline.Value.Date == DateTime.UtcNow.Date
                                   orderby t.TaskCreatedAt descending
                                   select t.TaskId).CountAsync();
                }
            }
            else
            {
                //all teams user joined
                var teams = await (from p in _dbContext.Participation.AsNoTracking()
                                   where p.ParticipationUserId == taskStatRequest.UserId && p.ParticipationIsDeleted != true
                                   select p.ParticipationTeamId).Distinct().ToListAsync();

                //all boards of teams
                var teamsBoards = await (from b in _dbContext.KanbanBoard.AsNoTracking()
                                         where teams.Contains(b.KanbanBoardTeamId)
                                         select b.KanbanBoardId).ToListAsync();

                if (taskStatRequest.StatusType == "todo")
                {
                    count = await (from t in _dbContext.Task.AsNoTracking()
                                   join kl in _dbContext.KanbanList.AsNoTracking() on t.TaskBelongedId equals kl.KanbanListId
                                   join b in _dbContext.KanbanBoard.AsNoTracking() on kl.KanbanListBoardBelongedId equals b.KanbanBoardId
                                   join ht in _dbContext.HandleTask.AsNoTracking() on t.TaskId equals ht.HandleTaskTaskId
                                   where teamsBoards.Contains(b.KanbanBoardId) && t.TaskIsDeleted != true
                                   && kl.KanbanListIsDeleted != true && t.TaskStatus == "todo" && ht.HandleTaskUserId == taskStatRequest.UserId
                                   orderby t.TaskCreatedAt descending
                                   select t.TaskId).CountAsync();
                }
                else
                {
                    count = await (from t in _dbContext.Task.AsNoTracking()
                                   join kl in _dbContext.KanbanList.AsNoTracking() on t.TaskBelongedId equals kl.KanbanListId
                                   join b in _dbContext.KanbanBoard.AsNoTracking() on kl.KanbanListBoardBelongedId equals b.KanbanBoardId
                                   join ht in _dbContext.HandleTask.AsNoTracking() on t.TaskId equals ht.HandleTaskTaskId
                                   where teamsBoards.Contains(b.KanbanBoardId) && t.TaskIsDeleted != true
                                   && kl.KanbanListIsDeleted != true && t.TaskDeadline.Value.Date == DateTime.UtcNow.Date
                                   && ht.HandleTaskUserId == taskStatRequest.UserId
                                   orderby t.TaskCreatedAt descending
                                   select t.TaskId).CountAsync();
                }
            }

            return count;
        }
        public async Task<List<int>> TasksReportCount(string userId)
        {
            var userTodo = await ReportCount(new TaskStatRequest
            {
                UserId = userId,
                OwnerType = "personal",
                StatusType = "todo",
            });

            var userDeadline = await ReportCount(new TaskStatRequest
            {
                UserId = userId,
                OwnerType = "personal",
                StatusType = "deadline",
            });

            var teamTodo = await ReportCount(new TaskStatRequest
            {
                UserId = userId,
                OwnerType = "team",
                StatusType = "todo",
            });

            var teamDeadline = await ReportCount(new TaskStatRequest
            {
                UserId = userId,
                OwnerType = "team",
                StatusType = "deadline",
            });

            var response = new List<int> { userTodo, userDeadline, teamTodo, teamDeadline };

            return response;
        }

        public async Task<List<TaskModalResponse>> TasksStatGet(TaskStatRequest taskStatRequest)
        {
            var response = new List<TaskModalResponse>();
            if (taskStatRequest.OwnerType == "personal")
            {
                if (taskStatRequest.StatusType == "todo")
                {
                    var tasks = await (from t in _dbContext.Task.AsNoTracking()
                                       join kl in _dbContext.KanbanList.AsNoTracking() on t.TaskBelongedId equals kl.KanbanListId
                                       join b in _dbContext.KanbanBoard.AsNoTracking() on kl.KanbanListBoardBelongedId equals b.KanbanBoardId
                                       where b.KanbanBoardUserId == taskStatRequest.UserId && t.TaskIsDeleted != true
                                       && kl.KanbanListIsDeleted != true && t.TaskStatus == "todo"
                                       orderby t.TaskCreatedAt descending
                                       select new
                                       {
                                           t.TaskId,
                                           t.TaskName,
                                           t.TaskStatus,
                                           t.TaskDeadline,
                                           t.TaskDescription,
                                           t.TaskImageUrl,
                                           b.KanbanBoardId
                                       }).ToListAsync();

                    response = tasks.Select(t => new TaskModalResponse
                    {
                        TaskId = t.TaskId,
                        TaskName = t.TaskName,
                        TaskStatus = t.TaskStatus,
                        TaskDeadline = t.TaskDeadline,
                        TaskDescription = t.TaskDescription,
                        TaskImage = t.TaskImageUrl,
                        Link = $"/managetask/mytasks?b={t.KanbanBoardId}&t={t.TaskId}",
                    }).ToList();
                }
                else
                {
                    var tasks = await (from t in _dbContext.Task.AsNoTracking()
                                       join kl in _dbContext.KanbanList.AsNoTracking() on t.TaskBelongedId equals kl.KanbanListId
                                       join b in _dbContext.KanbanBoard.AsNoTracking() on kl.KanbanListBoardBelongedId equals b.KanbanBoardId
                                       where b.KanbanBoardUserId == taskStatRequest.UserId && t.TaskIsDeleted != true
                                       && kl.KanbanListIsDeleted != true && t.TaskDeadline.Value.Date == DateTime.UtcNow.Date
                                       orderby t.TaskCreatedAt descending
                                       select new
                                       {
                                           t.TaskId,
                                           t.TaskName,
                                           t.TaskStatus,
                                           t.TaskDeadline,
                                           t.TaskDescription,
                                           t.TaskImageUrl,
                                           b.KanbanBoardId,
                                       }).ToListAsync();

                    response = tasks.Select(t => new TaskModalResponse
                    {
                        TaskId = t.TaskId,
                        TaskName = t.TaskName,
                        TaskStatus = t.TaskStatus,
                        TaskDeadline = t.TaskDeadline,
                        TaskDescription = t.TaskDescription,
                        TaskImage = t.TaskImageUrl,
                        Link = $"/managetask/mytasks?b={t.KanbanBoardId}&t={t.TaskId}",
                    }).ToList();
                }
            }
            else
            {
                //all teams user joined
                var teams = await (from p in _dbContext.Participation.AsNoTracking()
                                   where p.ParticipationUserId == taskStatRequest.UserId && p.ParticipationIsDeleted != true
                                   select p.ParticipationTeamId).Distinct().ToListAsync();

                //all boards of teams
                var teamsBoards = await (from b in _dbContext.KanbanBoard.AsNoTracking()
                                         where teams.Contains(b.KanbanBoardTeamId)
                                         select b.KanbanBoardId).ToListAsync();

                if (taskStatRequest.StatusType == "todo")
                {
                    var tasks = await (from t in _dbContext.Task.AsNoTracking()
                                       join kl in _dbContext.KanbanList.AsNoTracking() on t.TaskBelongedId equals kl.KanbanListId
                                       join b in _dbContext.KanbanBoard.AsNoTracking() on kl.KanbanListBoardBelongedId equals b.KanbanBoardId
                                       join ht in _dbContext.HandleTask.AsNoTracking() on t.TaskId equals ht.HandleTaskTaskId
                                       where teamsBoards.Contains(b.KanbanBoardId) && t.TaskIsDeleted != true
                                       && kl.KanbanListIsDeleted != true && t.TaskStatus == "todo"
                                       && ht.HandleTaskUserId == taskStatRequest.UserId
                                       orderby t.TaskCreatedAt descending
                                       select new
                                       {
                                           t.TaskId,
                                           t.TaskName,
                                           t.TaskStatus,
                                           t.TaskDeadline,
                                           t.TaskDescription,
                                           t.TaskImageUrl,
                                           b.KanbanBoardId,
                                           b.KanbanBoardTeamId
                                       }).ToListAsync();

                    response = tasks.Select(t => new TaskModalResponse
                    {
                        TaskId = t.TaskId,
                        TaskName = t.TaskName,
                        TaskStatus = t.TaskStatus,
                        TaskDeadline = t.TaskDeadline,
                        TaskDescription = t.TaskDescription,
                        TaskImage = t.TaskImageUrl,
                        Link = $"/managetask/teamtasks?gr={t.KanbanBoardTeamId}&b={t.KanbanBoardId}&t={t.TaskId}",
                    }).ToList();
                }
                else
                {
                    var tasks = await (from t in _dbContext.Task.AsNoTracking()
                                       join kl in _dbContext.KanbanList.AsNoTracking() on t.TaskBelongedId equals kl.KanbanListId
                                       join b in _dbContext.KanbanBoard.AsNoTracking() on kl.KanbanListBoardBelongedId equals b.KanbanBoardId
                                       join ht in _dbContext.HandleTask.AsNoTracking() on t.TaskId equals ht.HandleTaskTaskId
                                       where teamsBoards.Contains(b.KanbanBoardId) && t.TaskIsDeleted != true
                                       && kl.KanbanListIsDeleted != true && t.TaskDeadline.Value.Date == DateTime.UtcNow.Date
                                       && ht.HandleTaskUserId == taskStatRequest.UserId
                                       orderby t.TaskCreatedAt descending
                                       select new
                                       {
                                           t.TaskId,
                                           t.TaskName,
                                           t.TaskStatus,
                                           t.TaskDeadline,
                                           t.TaskDescription,
                                           t.TaskImageUrl,
                                           b.KanbanBoardId,
                                           b.KanbanBoardTeamId
                                       }).ToListAsync();

                    response = tasks.Select(t => new TaskModalResponse
                    {
                        TaskId = t.TaskId,
                        TaskName = t.TaskName,
                        TaskStatus = t.TaskStatus,
                        TaskDeadline = t.TaskDeadline,
                        TaskDescription = t.TaskDescription,
                        TaskImage = t.TaskImageUrl,
                        Link = $"/managetask/teamtasks?gr={t.KanbanBoardTeamId}&b={t.KanbanBoardId}&t={t.TaskId}",
                    }).ToList();
                }
            }

            return response;
        }
    }
}
