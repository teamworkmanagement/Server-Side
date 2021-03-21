﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Domain.Models.GroupChatUser;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IGroupChatUserRepository
    {
        Task<List<GroupChatUserResponse>> GetByUserId(string userId);
        Task<string> AddGroupChatUser(GroupChatUserRequest grChatUserReq);
        Task<bool> DeleteGroupChatUser(string grChatUserId);
    }
}
