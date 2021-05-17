using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.HandleTask;
using TeamApp.Application.DTOs.Task;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/handletask")]
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
            var res = await _repo.GetAllByTaskId(taskId);

            var outPut = new ApiResponse<List<HandleTaskResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpGet("byuserid/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var res = await _repo.GetAllByUserId(userId);

            var outPut = new ApiResponse<List<HandleTaskResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpPost]
        public async Task<IActionResult> AddHandleTask(HandleTaskRequest handleTaskReq)
        {
            var res = await _repo.AddHandleTask(handleTaskReq);

            var outPut = new ApiResponse<HandleTaskResponse>
            {
                Data = res,
                Succeeded = res == null ? false : true,
                Message = res == null ? "Thêm thất bại" : null,
            };

            return Ok(outPut);
        }

        [HttpPut("{handleTaskId}")]
        public async Task<IActionResult> UpdateHandleTask(string handleTaskId, [FromForm] HandleTaskRequest handleTaskReq)
        {
            var res = await _repo.UpdateHandleTask(handleTaskId, handleTaskReq);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res,
                Message = !res ? "Sửa thất bại" : null,
            };

            return Ok(outPut);
        }

        [HttpDelete("{handleTaskId}")]
        public async Task<IActionResult> DeleteHandleTask(string handleTaskId)
        {
            var res = await _repo.DeleteHandleTask(handleTaskId);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res,
                Message = !res ? "Xóa thất bại" : null,
            };

            return Ok(outPut);
        }

        [HttpPost("reassign-task")]
        public async Task<IActionResult> ReAssignTask(ReAssignModel reAssignModel)
        {
            var outPut = await _repo.ReAssignTask(reAssignModel);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }
    }
}
