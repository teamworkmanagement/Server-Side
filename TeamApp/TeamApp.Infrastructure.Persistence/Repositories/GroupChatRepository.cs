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
using TeamApp.Application.DTOs.User;
using System.Data;
using TeamApp.Infrastructure.Persistence.Helpers;
using Microsoft.AspNetCore.SignalR;
using TeamApp.Infrastructure.Persistence.Hubs.Chat;
using System.Collections.ObjectModel;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class GroupChatRepository : IGroupChatRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IHubContext<HubChatClient, IHubChatClient> _chatHub;
        public GroupChatRepository(TeamAppContext dbContext, IHubContext<HubChatClient, IHubChatClient> chatHub)
        {
            _dbContext = dbContext;
            _chatHub = chatHub;
        }
        public async Task<string> AddGroupChat(GroupChatRequest grChatReq)
        {
            var entity = new GroupChat
            {
                GroupChatId = string.IsNullOrEmpty(grChatReq.GroupChatId) ? Guid.NewGuid().ToString() : grChatReq.GroupChatId,
                GroupChatName = grChatReq.GroupChatName,
                GroupChatUpdatedAt = DateTime.UtcNow,
                GroupChatType = grChatReq.GroupChatType,
                GroupChatIsOfTeam = grChatReq.IsOfTeam,
            };

            await _dbContext.GroupChat.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.GroupChatId;
        }

        public async Task<string> AddGroupChatMembers(AddMembersRequest request)
        {
            var grc = await _dbContext.GroupChat.FindAsync(request.GroupChatId);

            if (grc == null)
                throw new KeyNotFoundException("Not found groupchat");

            if (grc.GroupChatType == "double")
            {
                var listUser = await (from gru in _dbContext.GroupChatUser.AsNoTracking()
                                      join gr in _dbContext.GroupChat.AsNoTracking() on gru.GroupChatUserGroupChatId equals gr.GroupChatId
                                      where gr.GroupChatId == grc.GroupChatId
                                      select gru.GroupChatUserUserId).ToListAsync();
                request.UserIds.AddRange(listUser);
                var grChatId = await AddGroupChatWithMembers(new GroupChatRequestMembers
                {
                    Members = request.UserIds,
                    //GroupChatName = Extensions.RadomString.RandomString(8),
                });

                return grChatId;

            }

            var listGrChatUsers = new List<GroupChatUser>();
            foreach (var user in request.UserIds)
            {
                listGrChatUsers.Add(new GroupChatUser
                {
                    GroupChatUserId = Guid.NewGuid().ToString(),
                    GroupChatUserUserId = user,
                    GroupChatUserGroupChatId = grc.GroupChatId,
                    GroupChatUserIsDeleted = false,
                });
            }

            await _dbContext.BulkInsertAsync(listGrChatUsers);

            return grc.GroupChatId;
        }

        public async Task<string> AddGroupChatWithMembers(GroupChatRequestMembers requestMembers)
        {
            if (requestMembers.Members.Count() == 2)
            {
                var userOne = requestMembers.Members[0];
                var userTwo = requestMembers.Members[1];

                var query = "SELECT g.group_chat_id " +
                        "FROM group_chat g " +
                        "JOIN group_chat_user gu ON (gu.group_chat_user_group_chat_id = g.group_chat_id) " +
                        "where g.group_chat_type='double' " +
                        "GROUP BY g.group_chat_id " +
                        $"HAVING (SUM(gu.group_chat_user_user_id IN ('{userOne}','{userTwo}')) = 2) " +
                        $"AND (SUM(gu.group_chat_user_user_id NOT IN ('{userOne}','{userTwo}')) = 0) ";

                var results = await RawQuery.RawSqlQuery(_dbContext, query
                    , x => (string)x[0]);

                if (results.Count() == 1)
                {
                    return results[0];
                }
            }

            var entity = new GroupChat
            {
                GroupChatId = string.IsNullOrEmpty(requestMembers.GroupChatId) ? Guid.NewGuid().ToString() : requestMembers.GroupChatId,
                GroupChatName = requestMembers.GroupChatName,
                GroupChatUpdatedAt = DateTime.UtcNow,
                GroupChatType = requestMembers.Members.Count() == 2 ? "double" : "multi",
            };

            await _dbContext.GroupChat.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            var listGrChatUsers = new List<GroupChatUser>();
            foreach (var user in requestMembers.Members)
            {
                listGrChatUsers.Add(new GroupChatUser
                {
                    GroupChatUserId = Guid.NewGuid().ToString(),
                    GroupChatUserUserId = user,
                    GroupChatUserGroupChatId = entity.GroupChatId,
                    GroupChatUserIsDeleted = false,
                });
            }

            await _dbContext.BulkInsertAsync(listGrChatUsers);

            var connections = await (from c in _dbContext.UserConnection.AsNoTracking()
                                     where requestMembers.Members.Contains(c.UserId)
                                     select c.ConnectionId).ToListAsync();
            var clients = new ReadOnlyCollection<string>(connections);


            var groupChatName = string.Empty;
            var partis = await (from p in _dbContext.GroupChatUser.AsNoTracking()
                                join u in _dbContext.User.AsNoTracking() on p.GroupChatUserUserId equals u.Id
                                where p.GroupChatUserGroupChatId == entity.GroupChatId
                                select u.FullName).Take(3).ToListAsync();
            if (partis.Count > 2)
            {
                groupChatName = $"{partis[0]} + {partis[2]} và ...";
            }

            var chatNamePush = string.IsNullOrEmpty(requestMembers.GroupChatName) ? groupChatName : requestMembers.GroupChatName;
            await _chatHub.Clients.Clients(clients).NewGroupChat(new GroupChatResponse
            {
                GroupChatId = entity.GroupChatId,
                GroupChatName = chatNamePush,
                GroupChatUpdatedAt = DateTime.UtcNow,
                GroupAvatar = $"https://ui-avatars.com/api/?name={chatNamePush}",
                LastestMes = null,
                GroupChatType = entity.GroupChatType
            });

            return entity.GroupChatId;
        }

        public async Task<object> CheckDoubleGroupChatExists(CheckDoubleGroupChatExists chatExists)
        {
            /*var lists = new List<string> { chatExists.UserOneId, chatExists.UserTwoId };
            var find = await (from gc in _dbContext.GroupChat.AsNoTracking()
                              join gru in _dbContext.GroupChatUser.AsNoTracking() on gc.GroupChatId equals gru.GroupChatUserGroupChatId
                              where gc.GroupChatType == "double" && lists.Contains(gru.GroupChatUserGroupChatId)
                              select gc).ToListAsync();*/

            var query = "SELECT g.group_chat_id " +
                        "FROM group_chat g " +
                        "JOIN group_chat_user gu ON (gu.group_chat_user_group_chat_id = g.group_chat_id) " +
                        "where g.group_chat_type='double' " +
                        "GROUP BY g.group_chat_id " +
                        $"HAVING (SUM(gu.group_chat_user_user_id IN ('{chatExists.UserOneId}','{chatExists.UserTwoId}')) = 2) " +
                        $"AND (SUM(gu.group_chat_user_user_id NOT IN ('{chatExists.UserOneId}','{chatExists.UserTwoId}')) = 0) ";
            var results = await RawQuery.RawSqlQuery(_dbContext, query
                , x => (string)x[0]);


            var user2 = await _dbContext.User.FindAsync(chatExists.UserTwoId);
            var user2Res = new UserResponse
            {
                UserId = user2.Id,
                UserFullname = user2.FullName,
                UserImageUrl = user2.ImageUrl,
            };
            if (results.Count() == 1)
            {
                return new
                {
                    Exists = true,
                    GroupChatId = results[0],
                    UserTwo = user2Res,
                };
            }

            return new
            {
                Exists = false,
                GroupChatId = string.Empty,
                UserTwo = user2Res,
            };
        }

        public async Task<bool> DeleteGroupChat(string grChatId)
        {
            return await Task.FromResult(false);
        }

        public async Task<CustomListGroupChatResponse> GetAllByUserId(GroupChatSearch search)
        {
            var responseCustom = new CustomListGroupChatResponse();

            var query = from gc in _dbContext.GroupChat.AsNoTracking()
                        join grc in _dbContext.GroupChatUser.AsNoTracking() on gc.GroupChatId equals grc.GroupChatUserGroupChatId
                        where grc.GroupChatUserIsDeleted == false
                        select new { gc, grc };

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
                    IsOfTeam = gr.gc.GroupChatIsOfTeam,
                    GroupChatType = gr.gc.GroupChatType,
                    GroupChatId = gr.gc.GroupChatId,
                    GroupChatName = gr.gc.GroupChatName,
                    GroupChatUpdatedAt = lastest == null ? gr.gc.GroupChatUpdatedAt.FormatTime() : lastest.MessageCreatedAt.FormatTime(),
                    NewMessage = gr.grc.GroupChatUserSeen,
                    GroupAvatar = gr.gc.GroupChatImageUrl,
                    LastestMes = lastest == null ? null : lastest.MessageType == "file" ? "[Tệp tin]" : lastest.MessageType == "image" ? "[Hình ảnh]" : lastest.MessageContent,
                });
            }

            lists = lists.OrderByDescending(x => x.GroupChatUpdatedAt).ToList();

            foreach (var l in lists)
            {
                if (l.GroupChatType == "double")
                {
                    var user = await (from grc in _dbContext.GroupChatUser.AsNoTracking()
                                      join u in _dbContext.User.AsNoTracking() on grc.GroupChatUserUserId equals u.Id
                                      where grc.GroupChatUserUserId != search.UserId && grc.GroupChatUserGroupChatId == l.GroupChatId
                                      select new { u.FullName, u.ImageUrl }).FirstOrDefaultAsync();

                    l.GroupChatName = !string.IsNullOrEmpty(l.GroupChatName) ? l.GroupChatName : user.FullName;
                    l.GroupAvatar = string.IsNullOrEmpty(l.GroupAvatar) ? (string.IsNullOrEmpty(user.ImageUrl) ? $"https://ui-avatars.com/api/?name={user.FullName}" : user.ImageUrl) : l.GroupAvatar;
                }
                else
                {
                    if (string.IsNullOrEmpty(l.GroupChatName))
                    {
                        var partis = await (from p in _dbContext.GroupChatUser.AsNoTracking()
                                            join u in _dbContext.User.AsNoTracking() on p.GroupChatUserUserId equals u.Id
                                            where p.GroupChatUserGroupChatId == l.GroupChatId
                                            select u.FullName).Take(3).ToListAsync();
                        if (partis.Count == 2)
                        {
                            l.GroupChatName = $"{partis[0]} và {partis[1]}";
                        }

                        else
                        {
                            l.GroupChatName = $"{partis[0]} + {partis[1]} và ...";
                        }
                    }

                    if (string.IsNullOrEmpty(l.GroupAvatar))
                    {
                        if (l.IsOfTeam)
                        {
                            if (l.GroupAvatar == null)
                            {
                                var team = await _dbContext.Team.FindAsync(l.GroupChatId);
                                l.GroupAvatar = string.IsNullOrEmpty(team.TeamImageUrl) ? $"https://ui-avatars.com/api/?name={l.GroupChatName}" : team.TeamImageUrl;
                            }
                        }
                        else
                            l.GroupAvatar = $"https://ui-avatars.com/api/?name={l.GroupChatName}";
                    }
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

            responseCustom.GroupChats = lists;

            if (lists.Where(x => x.GroupChatId == search.CurrentGroup).Count() > 0)
                responseCustom.CurrentGroup = search.CurrentGroup;
            else
            {
                if (lists.Count > 0)
                    responseCustom.CurrentGroup = lists[0].GroupChatId;
            }

            return responseCustom;
        }

        public async Task<bool> UpdateGroupChatImageUrl(GroupChatImageUpdateRequest grChatReq)
        {
            var entity = await _dbContext.GroupChat.FindAsync(grChatReq.GroupChatId);
            if (entity == null)
                return false;

            entity.GroupChatImageUrl = grChatReq.ImageUrl;

            _dbContext.GroupChat.Update(entity);

            var connections = from gru in _dbContext.GroupChatUser.AsNoTracking()
                              join d in _dbContext.UserConnection.AsNoTracking() on gru.GroupChatUserUserId equals d.UserId
                              where d.Type == "chat" && gru.GroupChatUserGroupChatId == grChatReq.GroupChatId
                              select d.ConnectionId;

            var query = await connections.AsNoTracking().ToListAsync();
            var clients = new ReadOnlyCollection<string>(query);
            await _chatHub.Clients.Clients(clients).ChangeGroupAvatar(grChatReq);

            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
