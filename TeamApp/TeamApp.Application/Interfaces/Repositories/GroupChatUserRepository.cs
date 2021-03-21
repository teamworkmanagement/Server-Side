using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Domain.Models.GroupChatUser;

namespace TeamApp.Application.Interfaces.Repositories
{
    public class GroupChatUserRepository : IGroupChatUserRepository
    {
        public Task<string> AddGroupChatUser(GroupChatUserRequest grChatUserReq)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteGroupChatUser(string grChatUserId)
        {
            throw new NotImplementedException();
        }

        public Task<List<GroupChatUserResponse>> GetByUserId(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
