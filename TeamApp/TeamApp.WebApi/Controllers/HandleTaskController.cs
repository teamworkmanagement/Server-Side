using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.HandleTask;
using TeamApp.Application.DTOs.Task;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/handletask")]
    public class HandleTaskController : ControllerBase
    {
        private readonly IHandleTaskRepository _repo;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public HandleTaskController(IHandleTaskRepository repo, IAuthenticatedUserService authenticatedUserService)
        {
            _repo = repo;
            _authenticatedUserService = authenticatedUserService;
        }

        /// <summary>
        /// Assign user in task API
        /// </summary>
        /// <param name="reAssignModel"></param>
        /// <returns></returns>
        [HttpPost("reassign-task")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> ReAssignTask(ReAssignModel reAssignModel)
        {
            reAssignModel.UserActionId = _authenticatedUserService.UserId;
            var outPut = await _repo.ReAssignTask(reAssignModel);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }
    }
}
