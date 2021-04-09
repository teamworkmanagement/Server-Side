﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using Task = System.Threading.Tasks.Task;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.DTOs.GroupChat;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class GroupChatRepository : IGroupChatRepository
    {
        private readonly TeamAppContext _dbContext;
        public GroupChatRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> AddGroupChat(GroupChatRequest grChatReq)
        {
            var entity = new GroupChat
            {
                GroupChatId = Guid.NewGuid().ToString(),
                GroupChatName = grChatReq.GroupChatName,
                GroupChatUpdatedAt = DateTime.UtcNow,
            };

            await _dbContext.GroupChat.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.GroupChatId;
        }

        public async Task<bool> DeleteGroupChat(string grChatId)
        {
            return await Task.FromResult(false);
        }

        public async Task<List<GroupChatResponse>> GetAllByUserId(string userId)
        {
            var query = from gc in _dbContext.GroupChat
                        join grc in _dbContext.GroupChatUser on gc.GroupChatId equals grc.GroupChatUserGroupChatId
                        select new { gc, grc };

            var outPut = query.Where(x => x.grc.GroupChatUserUserId == userId);


            return await outPut.Select(x => new GroupChatResponse
            {
                GroupChatId = x.gc.GroupChatId,
                GroupChatName = x.gc.GroupChatName,
                GroupChatUpdatedAt = x.gc.GroupChatUpdatedAt,
                NewMessage = x.grc.GroupChatUserSeen,
            }).ToListAsync();
        }

        public async Task<bool> UpdateGroupChat(string grchatId, GroupChatRequest grChatReq)
        {
            var entity = await _dbContext.GroupChat.FindAsync(grchatId);
            if (entity == null)
                return false;

            entity.GroupChatName = grChatReq.GroupChatName;
            entity.GroupChatUpdatedAt = grChatReq.GroupChatUpdatedAt;

            _dbContext.GroupChat.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}