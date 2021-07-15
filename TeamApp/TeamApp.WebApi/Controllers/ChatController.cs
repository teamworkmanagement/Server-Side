using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Message;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.Infrastructure.Persistence.Hubs.Chat;
using Task = System.Threading.Tasks.Task;

namespace TeamApp.WebApi.Controllers.Test
{
    [ApiController]
    [Authorize]
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

        /// <summary>
        /// Send messenger API
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost("messages")]
        public async Task Post(ChatMessage message)
        {
            // get list connections by group
            //get user of group => get list connections by user
            //chuyển tin nhắn cho các client

            var user = await (from gru in _dbContext.GroupChatUser.AsNoTracking()
                              where gru.GroupChatUserUserId == message.UserId && gru.GroupChatUserGroupChatId == message.GroupId
                              && gru.GroupChatUserIsDeleted == false
                              select gru).FirstOrDefaultAsync();

            if (user == null)
                throw new KeyNotFoundException("User not in this group chat");

            var connections = from gru in _dbContext.GroupChatUser.AsNoTracking()
                              join d in _dbContext.UserConnection.AsNoTracking() on gru.GroupChatUserUserId equals d.UserId
                              where d.Type == "chat" && gru.GroupChatUserGroupChatId == message.GroupId && gru.GroupChatUserIsDeleted == false
                              select d.ConnectionId;

            var query = await connections.AsNoTracking().Distinct().ToListAsync();
            var clients = new ReadOnlyCollection<string>(query);


            await _chatHub.Clients.Clients(clients).NhanMessage(message);


            var date = Application.Utils.Extensions.UnixTimeStampToDateTime(message.TimeSend);

            await _messageRepository.AddMessage(new MessageRequest
            {
                MessageUserId = message.UserId,
                MessageGroupChatId = message.GroupId,
                MessageContent = message.Message,
                MessageCreatedAt = date,
                MessageType = message.MessageType,
            });

            //await _chatHub.Clients.Groups(groupId).NhanMessage(message);
        }
    }
}
