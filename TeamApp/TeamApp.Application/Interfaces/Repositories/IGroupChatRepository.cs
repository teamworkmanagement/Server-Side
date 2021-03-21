using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Domain.Models.GroupChat;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IGroupChatRepository
    {
        Task<List<GroupChatResponse>> GetAllByUserId(string userId);
        Task<string> AddGroupChat(GroupChatRequest grChatReq);
        Task<bool> UpdateGroupChat(string grchatId, GroupChatRequest grChatReq);
        Task<bool> DeleteGroupChat(string grChatId);
    }
}
