using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                        $"where task.task_done_date >= '{startString}' and task.task_done_date < '{endString}') taskdone " +

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

            var query = "select user.user_fullname as UserFullName, count(*) as TaskDoneCount, sum(taskdoneboard.task_point) as Point "+

                        "from( "+
                        "select taskdone.task_id, taskdone.task_point "+
                        "from(select task.task_id, task.task_kanbanlist_id, task.task_point "+
                        "from task "+
                        "where task.task_done_date is not null ) taskdone "+

                        "where taskdone.task_kanbanlist_id in "+
                        "(select kanban_list.kanban_list_id "+
                        "from kanban_list "+
                        $"where kanban_list.kanban_list_belonged_id = '{statisticsRequest.BoardId}')) taskdoneboard "+

                        "join handle_task on handle_task.handle_task_task_id = taskdoneboard.task_id "+
                        "join user on handle_task.handle_task_user_id = user.user_id "+
                        "group by handle_task.handle_task_user_id";


            using (var connection = new MySqlConnection(connectionString))
            {
                var counts = await connection.QueryAsync<UsersTaskDoneAndPointResponse>(query);
                var outPut = counts.ToList();
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
                var count = await GetUserTaskDoneCount(new StatisticsRequest
                {
                    UserId = userTaskDoneRequest.UserId,
                    Filter = userTaskDoneRequest.Filter,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
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

        public async Task<List<int>> GetBoardTaskDone(BoardTaskDoneRequest boardTaskDoneRequest)
        {
            var board = await _dbContext.KanbanBoard.FindAsync(boardTaskDoneRequest.BoardId);
            if (board == null)
                throw new KeyNotFoundException("board not found");

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
    }
}
