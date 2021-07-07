using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Task;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/task")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _repo;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public TaskController(ITaskRepository repo, IAuthenticatedUserService authenticatedUserService)
        {
            _repo = repo;
            _authenticatedUserService = authenticatedUserService;
        }

        /// <summary>
        /// Add task API
        /// </summary>
        /// <param name="taskReq"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(ApiResponse<string>))]
        public async Task<IActionResult> AddTask(TaskRequest taskReq)
        {
            var res = await _repo.AddTask(taskReq);

            var outPut = new ApiResponse<string>
            {
                Data = res,
                Succeeded = res == null ? false : true,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Update task API
        /// </summary>
        /// <param name="taskReq"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> UpdateTask(TaskUpdateRequest taskReq)
        {
            var res = await _repo.UpdateTask(taskReq);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res,
                Message = !res ? "Update lỗi" : null,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Remove task API
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpDelete("{taskId}")]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> DeleteTask(string taskId)
        {
            var res = await _repo.DeleteTask(taskId);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res,
                Message = !res ? "Xóa lỗi" : null,
            };

            return Ok(outPut);
        }

        /*[HttpGet("{taskId}")]
        [ProducesDefaultResponseType(typeof(TaskResponse))]
        public async Task<IActionResult> GetTaskById(string taskId)
        {
            var outPut = await _repo.GetById(taskId);

            return Ok(new ApiResponse<TaskResponse>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut,
            });
        }*/

        /// <summary>
        /// Drag task API
        /// </summary>
        /// <param name="dragTaskModel"></param>
        /// <returns></returns>
        [HttpPost("drag-task")]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> DragTask(DragTaskModel dragTaskModel)
        {
            var outPut = await _repo.DragTask(dragTaskModel);
            return Ok(
                new ApiResponse<bool>
                {
                    Succeeded = outPut,
                    Data = outPut,
                });
        }

        /// <summary>
        /// Get task by filter API
        /// </summary>
        /// <param name="taskGetRequest"></param>
        /// <returns></returns>
        [HttpGet("boardtask")]
        [ProducesDefaultResponseType(typeof(ApiResponse<TaskResponse>))]
        public async Task<IActionResult> GetTaskByBoard([FromQuery] TaskGetRequest taskGetRequest)
        {
            var userId = _authenticatedUserService.UserId;
            var outPut = await _repo.GetTaskByBoard(userId, taskGetRequest);

            return Ok(new ApiResponse<TaskResponse>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut,
            });
        }
    }
}
