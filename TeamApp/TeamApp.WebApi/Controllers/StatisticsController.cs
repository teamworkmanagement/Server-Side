using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Statistics;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsRepository _repo;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public StatisticsController(IStatisticsRepository statisticsRepository, IAuthenticatedUserService authenticatedUserService)
        {
            _repo = statisticsRepository;
            _authenticatedUserService = authenticatedUserService;
        }

        [HttpGet("personal-task-done")]
        public async Task<IActionResult> GetPersonalTaskDone([FromQuery] UserTaskDoneRequest userTaskDoneRequest)
        {
            var outPut = await _repo.GetUserTaskDone(userTaskDoneRequest);
            return Ok(
                new ApiResponse<List<int>>
                {
                    Data = outPut,
                    Succeeded = true,
                });
        }

        [HttpGet("board-task-done")]
        public async Task<IActionResult> GetBoardTaskDone([FromQuery] BoardTaskDoneRequest boardTaskDoneRequest)
        {
            var outPut = await _repo.GetBoardTaskDone(boardTaskDoneRequest);
            return Ok(
                new ApiResponse<List<int>>
                {
                    Data = outPut,
                    Succeeded = true,
                });
        }

        [HttpGet("users-task-done-point-board")]
        public async Task<IActionResult> GetUsersTaskDoneAndPoint([FromQuery] UsersTaskDoneAndPointRequest usersTaskDoneAndPointRequest)
        {
            var outPut = await _repo.GetUsersTaskDoneAndPoint(usersTaskDoneAndPointRequest);

            return Ok(new ApiResponse<List<UsersTaskDoneAndPointResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        [HttpGet("user-task-done-boards")]
        public async Task<IActionResult> GetUsersTaskDoneInBoards([FromQuery] UserTaskDoneInBoardsRequest userTaskDoneInBoardsRequest)
        {
            var outPut = await _repo.GetUserTaskDoneInBoards(userTaskDoneInBoardsRequest);

            return Ok(new ApiResponse<List<int>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        [HttpPost("export-personalandteam")]
        public async Task<IActionResult> ExportPersonalAndTeamsTask([FromForm] ExportPersonalAndTeamsTaskRequest exportPersonal)
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = $"personalandteam_{Extensions.RadomString.RandomString(6)}.xlsx";
            byte[] data = await _repo.ExportPersonalAndTeamsTask(exportPersonal);

            return File(data, contentType, fileName);
        }

        [HttpPost("export-teamdoneboard")]
        public async Task<IActionResult> ExportTeamDoneBoard([FromForm] BoardDoneTaskExportRequest exportRequest)
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = $"teamdoneboard_{Extensions.RadomString.RandomString(6)}.xlsx";
            byte[] data = await _repo.ExportBoardDoneTask(exportRequest);

            return File(data, contentType, fileName);
        }

        [HttpPost("export-pointtask-groupbyuser")]
        public async Task<IActionResult> ExportTeamUserPointTask([FromForm] BoardPointAndDoneRequest pointAndDoneRequest)
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = $"pointtask_{Extensions.RadomString.RandomString(6)}.xlsx";
            byte[] data = await _repo.ExportUserBoardDonePointAndTask(pointAndDoneRequest);

            return File(data, contentType, fileName);
        }

        [HttpGet("tasks-status-count")]
        public async Task<IActionResult> TasksReportCount()
        {
            var outPut = await _repo.TasksReportCount(_authenticatedUserService.UserId);

            return Ok(new ApiResponse<List<int>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        [HttpGet("tasks-status-list")]
        public async Task<IActionResult> TasksStatGet([FromQuery] TaskStatRequest taskStatRequest)
        {
            taskStatRequest.UserId = _authenticatedUserService.UserId;
            var outPut = await _repo.TasksStatGet(taskStatRequest);

            return Ok(new ApiResponse<List<TaskModalResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }
    }
}
