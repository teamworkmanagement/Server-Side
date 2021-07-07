using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanList;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/kanbanlist")]
    public class KanbanListController : ControllerBase
    {
        private readonly IKanbanListRepository _repo;
        public KanbanListController(IKanbanListRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Add kanbanlist API
        /// </summary>
        /// <param name="kanbanListRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(ApiResponse<KanbanListUIResponse>))]
        public async Task<IActionResult> AddKanbanList(KanbanListRequest kanbanListRequest)
        {
            var outPut = await _repo.AddKanbanList(kanbanListRequest);

            return Ok(new ApiResponse<KanbanListUIResponse>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut,
            });
        }

        /// <summary>
        /// Remove kanbanlist API
        /// </summary>
        /// <param name="kanbanListRequest"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> RemoveKanbanList([FromQuery] KanbanListRequest kanbanListRequest)
        {
            var result = await _repo.RemoveList(kanbanListRequest);
            return Ok(new ApiResponse<bool>
            {
                Data = result,
                Succeeded = result,
            });
        }

        /// <summary>
        /// Change kanbanlist name API
        /// </summary>
        /// <param name="kanbanListChangeNameModel"></param>
        /// <returns></returns>
        [HttpPatch("name-list")]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeNameList(KanbanListChangeNameModel kanbanListChangeNameModel)
        {
            var result = await _repo.ChangeName(kanbanListChangeNameModel);
            return Ok(new ApiResponse<bool>
            {
                Data = result,
                Succeeded = result,
            });
        }
    }
}
