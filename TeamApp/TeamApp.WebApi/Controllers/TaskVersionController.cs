using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.TaskVersion;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/taskver")]
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
            var res = await _repo.GetById(taskVerId);

            var outPut = new ApiResponse<TaskVersionResponse>
            {
                Data = res,
                Succeeded = true,
                Message = res == null ? "Không tồn tại" : null,
            };

            return Ok(outPut);
        }

        [HttpGet("bytaskid/{taskId}")]
        public async Task<IActionResult> GetAllByTaskId(string taskId)
        {
            var res = await _repo.GetAllByTaskId(taskId);

            var outPut = new ApiResponse<List<TaskVersionResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }
    }
}
