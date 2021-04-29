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
    [Route("api/kanbanlist")]
    public class KanbanListController : ControllerBase
    {
        private readonly IKanbanListRepository _repo;
        public KanbanListController(IKanbanListRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public async Task<IActionResult> AddKanbanList(KanbanListRequest kanbanListRequest)
        {
            var outPut = await _repo.AddKanbanList(kanbanListRequest);

            return Ok(new ApiResponse<KanbanListResponse>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut,
            });
        }
    }
}
