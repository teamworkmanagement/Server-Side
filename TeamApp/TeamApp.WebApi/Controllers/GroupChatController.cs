using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.GroupChat;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupChatController : ControllerBase
    {
        private readonly IGroupChatRepository _repo;
        public GroupChatController(IGroupChatRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("byuserid/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            return Ok(await _repo.GetAllByUserId(userId));
        }

        [HttpPost]
        public async Task<IActionResult> AddGroupChat([FromForm] GroupChatRequest grChatReq)
        {
            return Ok(await _repo.AddGroupChat(grChatReq));
        }

        [HttpPut("{grChatId}")]
        public async Task<IActionResult> UpdateGroupChat(string grChatId, [FromForm] GroupChatRequest grChatReq)
        {
            return Ok(await _repo.UpdateGroupChat(grChatId, grChatReq));
        }

        [HttpDelete("{grChatId}")]
        public async Task<IActionResult> DeleteGroupChat(string grChatId)
        {
            return Ok(await _repo.DeleteGroupChat(grChatId));
        }
    }
}
