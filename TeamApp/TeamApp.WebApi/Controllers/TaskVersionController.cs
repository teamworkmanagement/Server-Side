using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskVersionController : ControllerBase
    {
        private readonly ITaskVersionRepository _repo;
        public TaskVersionController(ITaskVersionRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{taskVerId}")]
        public async Task<IActionResult> GetById(string taskVerId)
        {
            return Ok(await _repo.GetById(taskVerId));
        }

        [HttpGet("bytaskid/{taskId}")]
        public async Task<IActionResult> GetAllByTaskId(string taskId)
        {
            return Ok(await _repo.GetAllByTaskId(taskId));
        }

        [HttpDelete("{taskVerId}")]
        public async Task<IActionResult> DeleteById(string taskVerId)
        {
            return Ok(await _repo.DeleteById(taskVerId));
        }
    }
}
