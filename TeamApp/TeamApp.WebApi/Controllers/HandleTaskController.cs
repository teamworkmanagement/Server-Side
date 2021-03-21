using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.HandleTask;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HandleTaskController : ControllerBase
    {
        private readonly IHandleTaskRepository _repo;
        public HandleTaskController(IHandleTaskRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("bytaskid/{taskId}")]
        public async Task<IActionResult> GetAllByTaskId(string taskId)
        {
            return Ok(await _repo.GetAllByTaskId(taskId));
        }

        [HttpGet("byuserid/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            return Ok(await _repo.GetAllByUserId(userId));
        }

        [HttpPost]
        public async Task<IActionResult> AddHandleTask([FromForm] HandleTaskRequest handleTaskReq)
        {
            return Ok(await _repo.AddHandleTask(handleTaskReq));
        }

        [HttpPut("{handleTaskId}")]
        public async Task<IActionResult> UpdateHandleTask(string handleTaskId, [FromForm] HandleTaskRequest handleTaskReq)
        {
            return Ok(await _repo.UpdateHandleTask(handleTaskId, handleTaskReq));
        }

        [HttpDelete("{handleTaskId}")]
        public async Task<IActionResult> DeleteHandleTask(string handleTaskId)
        {
            return Ok(await _repo.DeleteHandleTask(handleTaskId));
        }
    }
}
