using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamApp.WebApi.Hubs.Chat
{
    public class HubChatClient : Hub<IHubChatClient>
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("Disconnected " + Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
