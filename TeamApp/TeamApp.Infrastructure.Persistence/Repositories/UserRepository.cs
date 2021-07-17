using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.User;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using Task = TeamApp.Infrastructure.Persistence.Entities.Task;
using TeamApp.Application.Utils;
using Microsoft.AspNetCore.SignalR;
using TeamApp.Infrastructure.Persistence.Hubs.App;
using TeamApp.Application.Utils;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IHubContext<HubAppClient, IHubAppClient> _hubApp;

        public UserRepository(TeamAppContext dbContext, IHubContext<HubAppClient, IHubAppClient> hubApp)
        {
            _dbContext = dbContext;
            _hubApp = hubApp;
        }

        public async Task<List<UserResponse>> GetAllUserInTeam(string userId, string teamId = null)
        {
            if (teamId != null)
            {
                var user = (from t in _dbContext.Team.AsNoTracking()
                            join p in _dbContext.Participation.AsNoTracking() on t.TeamId equals p.ParticipationTeamId
                            join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                            where t.TeamId == teamId && p.ParticipationIsDeleted == false
                            select new { u.Id, u.FullName, u.ImageUrl }).Distinct();

                return await user.Select(x => new UserResponse
                {
                    UserId = x.Id,
                    UserFullname = x.FullName,
                    UserImageUrl = string.IsNullOrEmpty(x.ImageUrl) ? $"https://ui-avatars.com/api/?name={x.FullName}" : x.ImageUrl,
                }).ToListAsync();
            }

            else
            {
                var teamList = from t in _dbContext.Team.AsNoTracking()
                               join par in _dbContext.Participation.AsNoTracking() on t.TeamId equals par.ParticipationTeamId
                               join u in _dbContext.User.AsNoTracking() on par.ParticipationUserId equals u.Id
                               where par.ParticipationIsDeleted == false
                               select new { u, t.TeamId, t.TeamName };

                //danh sách team mà user join
                teamList = teamList.Where(x => x.u.Id == userId).Distinct();

                var queryUser = await (from listTeam in teamList.AsNoTracking()
                                       join p in _dbContext.Participation.AsNoTracking() on listTeam.TeamId equals p.ParticipationTeamId
                                       join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                                       where p.ParticipationIsDeleted == false
                                       select new { u.Id, u.FullName, u.ImageUrl }).Distinct().ToListAsync();

                return queryUser.Select(x => new UserResponse
                {
                    UserId = x.Id,
                    UserFullname = x.FullName,
                    UserImageUrl = string.IsNullOrEmpty(x.ImageUrl) ? $"https://ui-avatars.com/api/?name={x.FullName}" : x.ImageUrl,
                }).ToList();
            }
        }

        public async Task<UserResponse> GetById(string userId)
        {
            var entity = await _dbContext.User.FindAsync(userId);
            if (entity == null)
                throw new KeyNotFoundException("User not found");

            var userRes = new UserResponse
            {
                UserId = entity.Id,
                UserEmail = entity.Email,
                UserFullname = entity.FullName,
                UserDateOfBirth = entity.Dob,
                UsePhoneNumber = entity.PhoneNumber,
                UserImageUrl = string.IsNullOrEmpty(entity.ImageUrl) ? $"https://ui-avatars.com/api/?name={entity.FullName}" : entity.ImageUrl,
                UserCreatedAt = entity.CreatedAt,
                UserDescription = entity.UserDescription,
                UserAddress = entity.UserAddress,
                UserGithubLink = entity.UserGithubLink,
                UserFacebookLink = entity.UserFacebookLink,
            };
            return userRes;
        }

        public async Task<List<UserResponse>> SearchUser(string userId, string keyWord, bool email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(keyWord);
                if (addr.Address == keyWord)
                    email = true;
                else
                    email = false;
            }
            catch
            {
                email = false;
            }
            var query = "";
            if (!email)
            {
                query = "SELECT * FROM user " +
                $"where user.user_id <> '{userId}' and user.user_fullname like '%{keyWord}%'";

                var newQuery = "select distinct u.user_id, u.user_fullname, u.user_image_url " +
                              "from user u " +
                              "join " +
                              "(select distinct p.participation_user_id " +
                              "from participation p " +
                              "join " +
                              "(select t.team_id, p.participation_user_id " +
                              "from team t join participation p on t.team_id = p.participation_team_id " +
                              $"where p.participation_user_id = '{userId}' and p.participation_is_deleted = 0) teamIDs " +
                              "on p.participation_team_id = teamIDs.team_id) userIDs " +
                              "on u.user_id = userIDs.participation_user_id " +
                              $"where u.user_fullname like '%{keyWord}%'";

                query = newQuery;
            }
            else
            {
                query = "SELECT * FROM user " +
                $"where user.user_id <> '{userId}' and user.user_email = '{keyWord}'";

                var newQuery = "select distinct u.user_id, u.user_fullname, u.user_image_url " +
                              "from user u " +
                              "join " +
                              "(select distinct p.participation_user_id " +
                              "from participation p " +
                              "join " +
                              "(select t.team_id, p.participation_user_id " +
                              "from team t join participation p on t.team_id = p.participation_team_id " +
                              $"where p.participation_user_id = '{userId}' and p.participation_is_deleted = 0) teamIDs " +
                              "on p.participation_team_id = teamIDs.team_id) userIDs " +
                              "on u.user_id = userIDs.participation_user_id " +
                              $"where u.user_email = '{keyWord}'";

                query = newQuery;

            }

            var listUsers = await Helpers.RawQuery.RawSqlQuery(_dbContext, query, (x) => new User
            {
                Id = (string)x[0],
                FullName = (string)x[1],
                ImageUrl = (x[2] == DBNull.Value) ? string.Empty : (string)x[2],
            });
            //Console.WriteLine(query);

            //var outPut = await _dbContext.User.FromSqlRaw(query).ToListAsync();

            return listUsers.Select(x => new UserResponse
            {
                UserId = x.Id,
                UserFullname = x.FullName,
                UserImageUrl = string.IsNullOrEmpty(x.ImageUrl) ? $"https://ui-avatars.com/api/?name={x.FullName}" : x.ImageUrl,
            }).ToList();
        }

        public async Task<List<UserResponse>> SearchUserAddToExistsChat(string userId, string grChatId, string keyWord, bool isEmail)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(keyWord);
                if (addr.Address == keyWord)
                    isEmail = true;
                else
                    isEmail = false;
            }
            catch
            {
                isEmail = false;
            }

            var query = "";
            if (!isEmail)
            {
                query = "SELECT * FROM user " +
                $"where user.user_id <> '{userId}' and user.user_fullname like '%{keyWord}%'";

                var newQuery = "select distinct u.user_id, u.user_fullname, u.user_image_url " +
                              "from user u " +
                              "join " +
                              "(select distinct p.participation_user_id " +
                              "from participation p " +
                              "join " +
                              "(select t.team_id, p.participation_user_id " +
                              "from team t join participation p on t.team_id = p.participation_team_id " +
                              $"where p.participation_user_id = '{userId}' and p.participation_is_deleted = 0) teamIDs " +
                              "on p.participation_team_id = teamIDs.team_id) userIDs " +
                              "on u.user_id = userIDs.participation_user_id " +
                              $"where u.user_fullname like '%{keyWord}%' and " +
                              $" u.user_id not in " +
                               "(select grc.group_chat_user_user_id " +
                                "from group_chat_user grc " +
                                $"where grc.group_chat_user_group_chat_id = '{grChatId}' and grc.group_chat_user_is_deleted = 0)";

                query = newQuery;
            }
            else
            {
                query = "SELECT * FROM user " +
                $"where user.user_id <> '{userId}' and user.user_email = '{keyWord}'";

                var newQuery = "select distinct u.user_id, u.user_fullname, u.user_image_url " +
                              "from user u " +
                              "join " +
                              "(select distinct p.participation_user_id " +
                              "from participation p " +
                              "join " +
                              "(select t.team_id, p.participation_user_id " +
                              "from team t join participation p on t.team_id = p.participation_team_id " +
                              $"where p.participation_user_id = '{userId}' and p.participation_is_deleted = 0) teamIDs " +
                              "on p.participation_team_id = teamIDs.team_id) userIDs " +
                              "on u.user_id = userIDs.participation_user_id " +
                              $"where u.user_email = '{keyWord}' and " +
                              $" u.user_id not in " +
                               "(select grc.group_chat_user_user_id " +
                                "from group_chat_user grc " +
                                $"where grc.group_chat_user_group_chat_id = '{grChatId}' and grc.group_chat_user_is_deleted = 0)";

                query = newQuery;

            }

            var listUsers = await Helpers.RawQuery.RawSqlQuery(_dbContext, query, (x) => new User
            {
                Id = (string)x[0],
                FullName = (string)x[1],
                ImageUrl = (x[2] == DBNull.Value) ? string.Empty : (string)x[2],
            });
            //Console.WriteLine(query);

            //var outPut = await _dbContext.User.FromSqlRaw(query).ToListAsync();

            return listUsers.Select(x => new UserResponse
            {
                UserId = x.Id,
                UserFullname = x.FullName,
                UserImageUrl = string.IsNullOrEmpty(x.ImageUrl) ? $"https://ui-avatars.com/api/?name={x.FullName}" : x.ImageUrl,
            }).ToList();
        }

        public async Task<List<UserResponse>> SearchUsersForKanban(UserKanbanSearchModel userKanbanSearch)
        {
            var kanbanBoard = await _dbContext.KanbanBoard.FindAsync(userKanbanSearch.BoardId);
            if (kanbanBoard == null)
                return null;

            var query = "select distinct user.user_id, user.user_fullname, user.user_image_url " +
                        "from participation " +
                        "join user on participation.participation_user_id = user.user_id " +
                        "where participation_team_id in " +
                        "(select kanban_board.kanban_board_teamid " +
                        "from kanban_board " +
                        $"where kanban_board.kanban_board_id = '{userKanbanSearch.BoardId}') "
                        + "and participation.participation_is_deleted = false";

            var listUsers = await Helpers.RawQuery.RawSqlQuery(_dbContext, query, (x) => new User
            {
                Id = (string)x[0],
                FullName = (string)x[1],
                ImageUrl = (x[2] == DBNull.Value) ? string.Empty : (string)x[2],
            });

            return listUsers.Select(x => new UserResponse
            {
                UserId = x.Id,
                UserFullname = x.FullName,
                UserImageUrl = string.IsNullOrEmpty(x.ImageUrl) ? $"https://ui-avatars.com/api/?name={x.FullName}" : x.ImageUrl,
            }).ToList();
        }

        public async Task<bool> UpdateUserImage(UpdateImageModel updateImageModel)
        {
            var user = await _dbContext.User.FindAsync(updateImageModel.UserId);
            if (user == null)
                throw new KeyNotFoundException("Not found user");

            if (updateImageModel.Delete)
                user.ImageUrl = null;
            else
                user.ImageUrl = updateImageModel.ImageUrl;

            _dbContext.User.Update(user);
            await _dbContext.SaveChangesAsync();

            var userInfo = new
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                UserAvatar = string.IsNullOrEmpty(user.ImageUrl) ? $"https://ui-avatars.com/api/?name={user.FullName}" : user.ImageUrl,
                UserDob = user.Dob.Value.Date,
                UserPhoneNumber = user.PhoneNumber,
                UserAddress = user.UserAddress,
                UserDescription = user.UserDescription,
                UserGithubLink = user.UserGithubLink,
                UserFacebookLink = user.UserFacebookLink,
                FirstTimeSocial = user.FirstTimeSocial,
            };

            var clients = await _dbContext.UserConnection.AsNoTracking()
                .Where(uc => uc.UserId == user.Id)
                .Select(uc => uc.ConnectionId)
                .ToListAsync();

            await _hubApp.Clients.Clients(clients).UpdateUserInfo(userInfo);

            return true;
        }

        public async Task<List<UserResponse>> SearchUsersForInviteMeeting(UserMeetingSearchModel userMeetingSearch)
        {
            var meeting = await _dbContext.Meeting.FindAsync(userMeetingSearch.MeetingId);

            //all users in meeting

            var usersMeeting = await (from mu in _dbContext.MeetingUser.AsNoTracking()
                                      where mu.MeetingId == meeting.MeetingId
                                      select mu.UserId).Distinct().ToListAsync();

            //all users in team
            var usersInTeam = await (from p in _dbContext.Participation.AsNoTracking()
                                     join u in _dbContext.User.AsNoTracking()
                                     on p.ParticipationUserId equals u.Id
                                     where p.ParticipationIsDeleted == false && p.ParticipationTeamId == meeting.TeamId
                                     select new { u.Id, u.FullName, u.ImageUrl }).Distinct().ToListAsync();

            //all users not join meeting
            var userNotJoin = usersInTeam.Where(x => !usersMeeting.Contains(x.Id)).ToList();

            var keyWord = userMeetingSearch.KeyWord.UnsignUnicode();

            var response = userNotJoin.Where(x => x.FullName.UnsignUnicode().Contains(keyWord)).Select(x => new UserResponse
            {
                UserId = x.Id,
                UserFullname = x.FullName,
                UserImageUrl = string.IsNullOrEmpty(x.ImageUrl) ? $"https://ui-avatars.com/api/?name={x.FullName}" : x.ImageUrl,
            }).ToList();

            return response;
        }
    }
}
