using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamApp.WebApi.Hubs.Chat
{
    public interface IHubChatClient
    {
        Task NhanMessage(ChatMessage message);
    }
}
