﻿using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("byuserid/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var res = await _repo.GetAllByUserId(userId);

            var outPut = new ApiResponse<List<TaskResponse>>
            {
                Data = res,
                Succeeded = false,
            };

            return Ok(outPut);
        }

        [HttpGet("byteamid/{teamId}")]
        public async Task<IActionResult> GetAllByTeamId(string teamId)
        {
            var res = await _repo.GetAllByTeamId(teamId);

            var outPut = new ApiResponse<List<TaskResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpGet("byteamid/{teamId}/byuserid/{userId}")]
        public async Task<IActionResult> GetAllByUserTeamId(string userId, string teamId)
        {
            var res = await _repo.GetAllByUserTeamId(userId, teamId);

            var outPut = new ApiResponse<List<TaskResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaging([FromQuery] RequestParameter parameter)
        {
            var res = await _repo.GetPaging(parameter);

            var outPut = new ApiResponse<PagedResponse<TaskResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpPost]
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

        [HttpPut]
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

        [HttpDelete("{taskId}")]
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

        [HttpGet("{taskId}")]
        [ProducesDefaultResponseType(typeof(TaskResponse))]
        public async Task<IActionResult> GetTaskById(string taskId)
        {
            var outPut = await _repo.GetById(taskId);

            return Ok(new ApiResponse<TaskResponse>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut,
            });
        }

        [HttpPost("drag-task")]
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

        [HttpGet("boardtask")]
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
