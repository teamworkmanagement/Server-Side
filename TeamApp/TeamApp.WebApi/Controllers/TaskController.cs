using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Task;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _repo;
        public TaskController(ITaskRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("byuserid/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            return Ok(await _repo.GetAllByUserId(userId));
        }

        [HttpGet("byteamid/{teamId}")]
        public async Task<IActionResult> GetAllByTeamId(string teamId)
        {
            return Ok(await _repo.GetAllByTeamId(teamId));
        }

        [HttpGet("byteamid/{teamId}/byuserid/{userId}")]
        public async Task<IActionResult> GetAllByUserTeamId(string userId, string teamId)
        {
            return Ok(await _repo.GetAllByUserTeamId(userId, teamId));
        }

        [HttpGet]
        public async Task<IActionResult> GetPaging([FromQuery] RequestParameter parameter)
        {
            return Ok(await _repo.GetPaging(parameter));
        }

        [HttpPost]
        public async Task<IActionResult> AddTask([FromForm] TaskRequest taskReq)
        {
            return Ok(await _repo.AddTask(taskReq));
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(string taskId, [FromForm] TaskRequest taskReq)
        {
            return Ok(await _repo.UpdateTask(taskId, taskReq));
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(string taskId)
        {
            return Ok(await _repo.DeleteTask(taskId));
        }
    }
}
