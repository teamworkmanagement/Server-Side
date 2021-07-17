using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Meeting;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using TeamApp.Infrastructure.Persistence.Hubs.App;
using TeamApp.Application.Utils;
using Newtonsoft.Json;
using TeamApp.Infrastructure.Persistence.Hubs.Notification;
using TeamApp.Application.DTOs.Notification;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IHubContext<HubAppClient, IHubAppClient> _hubApp;
        private readonly IHubContext<HubNotificationClient, IHubNotificationClient> _hubNoti;
        public MeetingRepository(TeamAppContext dbContext, IHubContext<HubAppClient, IHubAppClient> hubApp, IHubContext<HubNotificationClient, IHubNotificationClient> hubNoti)
        {
            _dbContext = dbContext;
            _hubApp = hubApp;
            _hubNoti = hubNoti;
        }
        public async Task<MeetingResponse> AddMeeting(MeetingRequest meetingRequest)
        {
            var meeting = new Meeting
            {
                MeetingId = Guid.NewGuid().ToString(),
                UserCreateId = meetingRequest.UserCreateId,
                MeetingName = meetingRequest.MeetingName,
                TeamId = meetingRequest.TeamId,
                Status = "meeting",
                Password = Extensions.RadomString.RandomString(8),
            };

            await _dbContext.Meeting.AddAsync(meeting);
            await _dbContext.SaveChangesAsync();

            var meetingUser = new MeetingUser
            {
                Id = Guid.NewGuid().ToString(),
                UserId = meetingRequest.UserCreateId,
                UserConnectionId = meetingRequest.ConnectionId,
                MeetingId = meeting.MeetingId,
            };

            await _dbContext.AddAsync(meetingUser);
            await _dbContext.SaveChangesAsync();

            var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                 join uc in _dbContext.UserConnection.AsNoTracking()
                                 on p.ParticipationUserId equals uc.UserId
                                 where p.ParticipationIsDeleted == false && p.ParticipationTeamId == meetingRequest.TeamId
                                 select uc.ConnectionId).ToListAsync();

            await _hubApp.Clients.Clients(clients).CreateMeeting(new
            {
                meetingRequest.TeamId,
            });

            return new MeetingResponse
            {
                MeetingId = meeting.MeetingId,
                UserCreateId = meetingRequest.UserCreateId,
                MeetingName = meeting.MeetingName,
                TeamId = meetingRequest.TeamId,
                Status = "meeting",
                Password = meeting.Password,
            };
        }

        public async Task<bool> DeleteMeeting(string meetingId)
        {
            var meeting = await _dbContext.Meeting.FindAsync(meetingId);
            if (meeting == null)
                throw new KeyNotFoundException("Not found meeting");

            _dbContext.Remove(meeting);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<MeetingResponse>> GetMeetingByTeam(string teamId)
        {
            var meetings = await (from m in _dbContext.Meeting.AsNoTracking()
                                  join t in _dbContext.Team.AsNoTracking() on m.TeamId equals t.TeamId
                                  join u in _dbContext.User.AsNoTracking() on m.UserCreateId equals u.Id
                                  where m.TeamId == teamId
                                  select new { m, t.TeamName, u.FullName, u.ImageUrl }).ToListAsync();

            return meetings.Select(m => new MeetingResponse
            {
                MeetingId = m.m.MeetingId,
                MeetingName = m.m.MeetingName,
                UserCreateId = m.m.UserCreateId,
                UserCreateName = m.FullName,
                UserCreateAvatar = m.ImageUrl,
                CreatedAt = m.m.CreatedAt,
                TeamId = m.m.TeamId,
                TeamName = m.TeamName,
                Status = m.m.Status,
                Password = m.m.Password,
            }).ToList();
        }

        public async Task<bool> JoinMeeting(JoinMeetingModel joinMeetingModel)
        {
            var meetingUser = await (from m in _dbContext.Meeting.AsNoTracking()
                                     join mu in _dbContext.MeetingUser.AsNoTracking()
                                     on m.MeetingId equals mu.MeetingId
                                     where mu.UserId == joinMeetingModel.UserId
                                     select mu).FirstOrDefaultAsync();

            //đã join = 1 connection khác
            if (meetingUser != null && meetingUser.UserConnectionId == joinMeetingModel.UserConnectionId)
            {
                return true;
            }

            if (meetingUser != null && meetingUser.UserConnectionId != joinMeetingModel.UserConnectionId)
            {
                return false;
            }

            meetingUser = new MeetingUser
            {
                Id = Guid.NewGuid().ToString(),
                UserId = joinMeetingModel.UserId,
                UserConnectionId = joinMeetingModel.UserConnectionId,
                MeetingId = joinMeetingModel.MeetingId,
            };

            await _dbContext.AddAsync(meetingUser);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> LeaveMeeting(LeaveMeetingModel leaveMeetingModel)
        {
            var meetingUser = await (from mu in _dbContext.MeetingUser
                                     where mu.MeetingId == leaveMeetingModel.MeetingId
                                     && mu.UserId == leaveMeetingModel.UserId
                                     select mu).FirstOrDefaultAsync();

            if (meetingUser == null)
                return false;

            _dbContext.Remove(meetingUser);
            await _dbContext.SaveChangesAsync();

            var meetingUsers = await (from mu in _dbContext.MeetingUser.AsNoTracking()
                                      where mu.MeetingId == leaveMeetingModel.MeetingId
                                      select mu.Id).ToListAsync();
            //họp kết thúc
            if (meetingUsers.Count == 0)
            {
                var meeting = await (from m in _dbContext.Meeting
                                     where m.MeetingId == leaveMeetingModel.MeetingId
                                     select m).FirstOrDefaultAsync();

                _dbContext.Remove(meeting);
                await _dbContext.SaveChangesAsync();

                //push noti rt
                var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                     join uc in _dbContext.UserConnection.AsNoTracking()
                                     on p.ParticipationUserId equals uc.UserId
                                     where p.ParticipationIsDeleted == false && p.ParticipationTeamId == meeting.TeamId
                                     select uc.ConnectionId).Distinct().ToListAsync();

                await _hubApp.Clients.Clients(clients).RemoveMeeting(new
                {
                    MeetingId = meeting.MeetingId,
                    TeamId = meeting.TeamId,
                });
            }
            return true;
        }
        public async Task<bool> InviteMembers(InviteMemberModel inviteMemberModel)
        {
            var notiGroup = Guid.NewGuid().ToString();

            var notis = new List<Notification>();
            var inviteUser = await _dbContext.User.FindAsync(inviteMemberModel.UserInvite);
            foreach (var u in inviteMemberModel.UserIds)
            {
                notis.Add(new Notification
                {
                    NotificationId = Guid.NewGuid().ToString(),
                    NotificationCreatedAt = DateTime.UtcNow,
                    NotificationActionUserId = inviteMemberModel.UserInvite,
                    NotificationGroup = notiGroup,
                    NotificationContent = "đã mời bạn tham gia 1 cuộc họp",
                    NotificationStatus = false,
                    NotificationLink = JsonConvert.SerializeObject(new { MeetingId = inviteMemberModel.MeetingId }),
                    NotificationIsDeleted = false,
                    NotificationUserId = u,
                });
            }

            await _dbContext.BulkInsertAsync(notis);

            var actionUser = await _dbContext.User.FindAsync(inviteMemberModel.UserInvite);

            var clients = await (from uc in _dbContext.UserConnection.AsNoTracking()
                                 where inviteMemberModel.UserIds.Contains(uc.UserId)
                                 select uc.ConnectionId).ToListAsync();

            await _hubNoti.Clients.Clients(clients).SendNoti(new NotificationResponse
            {
                NotificationActionFullName = actionUser.FullName,
                NotificationActionAvatar = string.IsNullOrEmpty(actionUser.ImageUrl) ? $"https://ui-avatars.com/api/?name={actionUser.FullName}" : actionUser.ImageUrl,
                NotificationGroup = notiGroup,
                NotificationContent = "đã mời bạn tham gia 1 cuộc họp",
                NotificationStatus = false,
                NotificationLink = JsonConvert.SerializeObject(new { MeetingId = inviteMemberModel.MeetingId }),
            });

            return true;
        }

        public async Task<MeetingResponse> GetById(string meetingId)
        {
            var meeting = await (from m in _dbContext.Meeting.AsNoTracking()
                                 join u in _dbContext.User.AsNoTracking() on m.UserCreateId equals u.Id
                                 where m.MeetingId == meetingId
                                 select new { m, u.FullName, u.ImageUrl }).FirstOrDefaultAsync();

            if (meeting == null)
            {
                throw new KeyNotFoundException("Meeting not found");
            }

            return new MeetingResponse
            {
                MeetingId = meeting.m.MeetingId,
                MeetingName = meeting.m.MeetingName,
                UserCreateId = meeting.m.UserCreateId,
                UserCreateName = meeting.FullName,
                UserCreateAvatar = meeting.ImageUrl,
                CreatedAt = meeting.m.CreatedAt,
                TeamId = meeting.m.TeamId,
                Status = meeting.m.Status,
                Password = meeting.m.Password,
            };
        }
    }
}
