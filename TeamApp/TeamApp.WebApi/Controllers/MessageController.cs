using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Message;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/message")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _repo;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public MessageController(IMessageRepository repo, IAuthenticatedUserService authenticatedUserService)
        {
            _repo = repo;
            _authenticatedUserService = authenticatedUserService;
        }

        /// <summary>
        /// Get pagination messages data API
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(ApiResponse<PagedResponse<MessageResponse>>))]
        public async Task<IActionResult> GetPaging([FromQuery] MessageRequestParameter parameter)
        {
            parameter.UserId = _authenticatedUserService.UserId;
            var res = await _repo.GetPaging(parameter);

            var outPut = new ApiResponse<PagedResponse<MessageResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }
    }
}
