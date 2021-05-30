using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.GroupChat;

namespace TeamApp.Infrastructure.Persistence.Hubs.Chat
{
    public interface IHubChatClient
    {
        Task NhanMessage(ChatMessage message);
        Task NewGroupChat(GroupChatResponse groupChatResponse);
    }
}
