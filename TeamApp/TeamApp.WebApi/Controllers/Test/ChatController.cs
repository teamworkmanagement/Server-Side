using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.WebApi.Hubs.Chat;

namespace TeamApp.WebApi.Controllers.Test
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<HubChatClient, IHubChatClient> _chatHub;

        public ChatController(IHubContext<HubChatClient, IHubChatClient> chatHub)
        {
            _chatHub = chatHub;
        }


        [HttpPost("messages")]
        public async Task Post(ChatMessage message)
        {
            // run some logic...
            //chuyển tin nhắn cho các client

            await _chatHub.Clients.All.NhanMessage(message);
        }
    }
}
