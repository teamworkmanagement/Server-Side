using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Domain.Models.GroupChat;

namespace TeamApp.Application.Interfaces.Repositories
{
    public class GroupChatRepository : IGroupChatRepository
    {
        public Task<string> AddGroupChat(GroupChatRequest grChatReq)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteGroupChat(string grChatId)
        {
            throw new NotImplementedException();
        }

        public Task<List<GroupChatResponse>> GetAllByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateGroupChat(string grchatId, GroupChatRequest grChatReq)
        {
            throw new NotImplementedException();
        }
    }
}
