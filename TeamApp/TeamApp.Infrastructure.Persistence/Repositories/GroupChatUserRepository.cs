using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.GroupChatUser;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.Application.Utils;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class GroupChatUserRepository : IGroupChatUserRepository
    {
        private readonly TeamAppContext _dbContext;

        public GroupChatUserRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> AddGroupChatUser(GroupChatUserRequest grChatUserReq)
        {
            var entity = new GroupChatUser
            {
                GroupChatUserId = Guid.NewGuid().ToString(),
                GroupChatUserUserId = grChatUserReq.GroupChatUserUserId,
                GroupChatUserGroupChatId = grChatUserReq.GroupChatUserGroupChatId,
                GroupChatUserIsDeleted = false,
            };

            await _dbContext.GroupChatUser.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.GroupChatUserId;
        }

        public async Task<bool> DeleteGroupChatUser(string groupId, string userId)
        {
            var entity = await _dbContext.GroupChatUser.Where(x => x.GroupChatUserUserId == userId && x.GroupChatUserGroupChatId == groupId).FirstOrDefaultAsync();

            if (entity == null)
                return false;

            entity.GroupChatUserIsDeleted = true;

            _dbContext.GroupChatUser.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;

            //Cần xem xét thêm khi mà user đã bị xóa khỏi nhóm, bật lại cờ không add
        }

        public async Task<List<GroupChatUserResponse>> GetByUserId(string userId)
        {
            var query = from grc in _dbContext.GroupChatUser
                        where grc.GroupChatUserUserId == userId
                        select grc;

            return await query.Select(x => new GroupChatUserResponse
            {
                GroupChatUserId = x.GroupChatUserId,
                GroupChatUserUserId = x.GroupChatUserUserId,
                GroupChatUserGroupChatId = x.GroupChatUserGroupChatId,
                GroupChatUserIsDeleted = x.GroupChatUserIsDeleted,
            }).ToListAsync();
        }
    }
}
