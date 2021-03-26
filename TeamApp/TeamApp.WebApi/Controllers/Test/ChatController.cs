using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.WebApi.Hubs.Chat;
using Task = System.Threading.Tasks.Task;

namespace TeamApp.WebApi.Controllers.Test
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly TeamAppContext _dbContext;
        private readonly IHubContext<HubChatClient, IHubChatClient> _chatHub;

        public ChatController(IHubContext<HubChatClient, IHubChatClient> chatHub, TeamAppContext dbContext)
        {
            _chatHub = chatHub;
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpPost("messages")]
        public async Task Post(ChatMessage message)
        {
            // get list connections by group
            //get user of group => get list connections by user
            //chuyển tin nhắn cho các client
            var groupId = message.GroupId;
            var nhomTv = from a in _dbContext.GroupChat
                         join b in _dbContext.GroupChatUser on a.GroupChatId equals b.GroupChatUserGroupChatId
                         join c in _dbContext.User on b.GroupChatUserUserId equals c.Id
                         join d in _dbContext.UserConnection on c.Id equals d.UserId
                         select new { a, d };

            var query = await nhomTv.Where(x => x.a.GroupChatId == groupId).ToListAsync();


            foreach (var f in query)
            {
                if (f.d.UserId != message.UserId)
                {
                    Console.WriteLine("shit " + f.d.ConnectionId);
                    await _chatHub.Groups.AddToGroupAsync(f.d.ConnectionId, groupId);
                }    
                    
            }

            await _chatHub.Clients.Groups(groupId).NhanMessage(message);
        }
    }
}
