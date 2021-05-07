using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Message;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.WebApi.Hubs.Chat;
using Task = System.Threading.Tasks.Task;

namespace TeamApp.WebApi.Controllers.Test
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly TeamAppContext _dbContext;
        private readonly IHubContext<HubChatClient, IHubChatClient> _chatHub;
        private readonly IMessageRepository _messageRepository;

        public ChatController(IHubContext<HubChatClient, IHubChatClient> chatHub, TeamAppContext dbContext, IMessageRepository messageRepository)
        {
            _chatHub = chatHub;
            _dbContext = dbContext;
            _messageRepository = messageRepository;
        }

        [Authorize]
        [HttpPost("messages")]
        public async Task Post(ChatMessage message)
        {
            // get list connections by group
            //get user of group => get list connections by user
            //chuyển tin nhắn cho các client
            var groupId = message.GroupId;
            var nhomTv = from a in _dbContext.GroupChat.AsNoTracking()
                         join b in _dbContext.GroupChatUser.AsNoTracking() on a.GroupChatId equals b.GroupChatUserGroupChatId
                         join c in _dbContext.User.AsNoTracking() on b.GroupChatUserUserId equals c.Id
                         join d in _dbContext.UserConnection.AsNoTracking() on c.Id equals d.UserId
                         select new { a, d };

            var query = await nhomTv.AsNoTracking().Where(x => x.a.GroupChatId == groupId).ToListAsync();

            foreach (var f in query)
            {
                if (f.d.UserId != message.UserId)
                {
                    //await _chatHub.Groups.AddToGroupAsync(f.d.ConnectionId, groupId);
                    _chatHub.Clients.Client(f.d.ConnectionId).NhanMessage(message);
                }
            }

            var date = Application.Utils.Extensions.UnixTimeStampToDateTime(message.TimeSend);

            await _messageRepository.AddMessage(new MessageRequest
            {
                MessageUserId = message.UserId,
                MessageGroupChatId = message.GroupId,
                MessageContent = message.Message,
                MessageCreatedAt = date,
                MessageType=message.MessageType,
            });

            //await _chatHub.Clients.Groups(groupId).NhanMessage(message);
        }
    }
}
