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
                GroupChatType = grChatReq.GroupChatType,
            };

            await _dbContext.GroupChat.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.GroupChatId;
        }

        public async Task<bool> DeleteGroupChat(string grChatId)
        {
            return await Task.FromResult(false);
        }

        public async Task<List<GroupChatResponse>> GetAllByUserId(GroupChatSearch search)
        {
            var query = from gc in _dbContext.GroupChat.AsNoTracking()
                        join grc in _dbContext.GroupChatUser.AsNoTracking() on gc.GroupChatId equals grc.GroupChatUserGroupChatId
                        join t in _dbContext.Team.AsNoTracking() on gc.GroupChatId equals t.TeamId
                        select new { gc, grc, t.TeamImageUrl };

            var outputQuery = query.Where(x => x.grc.GroupChatUserUserId == search.UserId).OrderByDescending(x => x.gc.GroupChatUpdatedAt);

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
                    GroupChatType = gr.gc.GroupChatType,
                    GroupChatId = gr.gc.GroupChatId,
                    GroupChatName = gr.gc.GroupChatName,
                    GroupChatUpdatedAt = lastest == null ? null : lastest.MessageCreatedAt.FormatTime(),
                    NewMessage = gr.grc.GroupChatUserSeen,
                    GroupAvatar = string.IsNullOrEmpty(gr.TeamImageUrl) ? $"https://ui-avatars.com/api/?name={gr.gc.GroupChatName}" : gr.TeamImageUrl,
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

            foreach (var l in lists)
            {
                if (l.GroupChatType == "double")
                {
                    var user = await (from grc in _dbContext.GroupChatUser.AsNoTracking()
                                      join u in _dbContext.User.AsNoTracking() on grc.GroupChatUserUserId equals u.Id
                                      where grc.GroupChatUserUserId != search.UserId && grc.GroupChatUserGroupChatId == l.GroupChatId
                                      select new { u.FullName, u.ImageUrl }).FirstOrDefaultAsync();

                    l.GroupChatName = user.FullName;
                    l.GroupAvatar = string.IsNullOrEmpty(user.ImageUrl) ? $"https://ui-avatars.com/api/?name={user.FullName}" : user.ImageUrl;
                }

            }


            if (search.IsSearch)
            {
                if (!string.IsNullOrEmpty(search.KeyWord))
                {
                    var xoadau = search.KeyWord.UnsignUnicode();
                    lists = lists.Where(x => x.GroupChatName.UnsignUnicode().Contains(xoadau)).Select(x => x).ToList();
                }
            }
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
