using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using Task = System.Threading.Tasks.Task;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.DTOs.GroupChat;
using TeamApp.Application.Utils;

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
                GroupChatId = string.IsNullOrEmpty(grChatReq.GroupChatId) ? Guid.NewGuid().ToString() : grChatReq.GroupChatId,
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
            var query = from gc in _dbContext.GroupChat.AsNoTracking()
                        join grc in _dbContext.GroupChatUser.AsNoTracking() on gc.GroupChatId equals grc.GroupChatUserGroupChatId
                        select new { gc, grc };

            var outputQuery = query.Where(x => x.grc.GroupChatUserUserId == userId).OrderByDescending(x => x.gc.GroupChatUpdatedAt);

            var outPut = await outputQuery.ToListAsync();

            var lists = new List<GroupChatResponse>();
            foreach (var gr in outPut)
            {
                //get lastest message
                var lastest = await (from m in _dbContext.Message.AsNoTracking()
                                     where m.MessageGroupChatId == gr.gc.GroupChatId
                                     orderby m.MessageCreatedAt descending
                                     select m).FirstOrDefaultAsync();

                lists.Add(new GroupChatResponse
                {
                    GroupChatId = gr.gc.GroupChatId,
                    GroupChatName = gr.gc.GroupChatName,
                    GroupChatUpdatedAt = lastest == null ? null : lastest.MessageCreatedAt.FormatTime(),
                    NewMessage = gr.grc.GroupChatUserSeen,
                    LastestMes = lastest == null ? null : lastest.MessageType == "file" ? "[Tệp tin]" : lastest.MessageType == "image" ? "[Hình ảnh]" : lastest.MessageContent,
                });
            }

            /*return await outputQuery.Select(x => new GroupChatResponse
            {
                GroupChatId = x.gc.GroupChatId,
                GroupChatName = x.gc.GroupChatName,
                GroupChatUpdatedAt = x.gc.Message.OrderByDescending(x => x.MessageCreatedAt).FirstOrDefault().MessageCreatedAt.FormatTime(),
                NewMessage = x.grc.GroupChatUserSeen,
                LastestMes = x.gc.Message.OrderByDescending(x => x.MessageCreatedAt).FirstOrDefault().MessageContent,
            }).ToListAsync();*/
            lists = lists.OrderByDescending(x => x.GroupChatUpdatedAt).ToList();

            return lists;
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
