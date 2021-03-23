using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Message;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _repo;
        public MessageController(IMessageRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("byuserid/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            return Ok(await _repo.GetAllByUserId(userId));
        }

        [HttpGet]
        public async Task<IActionResult> GetPaging([FromQuery] RequestParameter parameter)
        {
            return Ok(await _repo.GetPaging(parameter));
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage([FromForm] MessageRequest msgReq)
        {
            return Ok(await _repo.AddMessage(msgReq));
        }

        [HttpDelete("{msgId}")]
        public async Task<IActionResult> DeleteMessage(string msgId)
        {
            return Ok(await _repo.DeleteMessage(msgId));
        }
    }
}
