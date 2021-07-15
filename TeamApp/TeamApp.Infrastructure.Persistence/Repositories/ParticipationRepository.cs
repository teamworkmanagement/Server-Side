using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Paricipation;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.Application.Utils;
using TeamApp.Application.Exceptions;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using TeamApp.Infrastructure.Persistence.Hubs.Notification;
using TeamApp.Infrastructure.Persistence.Hubs.App;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class ParticipationRepository : IParticipationRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly INotificationRepository _notiRepo;
        private readonly IHubContext<HubAppClient, IHubAppClient> _hubApp;
        public ParticipationRepository(TeamAppContext dbContext, INotificationRepository notiRepo, IHubContext<HubAppClient, IHubAppClient> hubApp)
        {
            _dbContext = dbContext;
            _notiRepo = notiRepo;
            _hubApp = hubApp;
        }

        public async Task<ParticipationResponse> AddParticipation(ParticipationRequest participationRequest)
        {
            var entity = new Participation();
            User user = null;
            if (participationRequest.IsByEmail)
            {
                user = await (from u in _dbContext.User.AsNoTracking()
                              where u.Email == participationRequest.Email
                              select u).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new KeyNotFoundException("No user found");
                }

                var backupUser = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(user));

                user = await (from p in _dbContext.Participation.AsNoTracking()
                              join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                              where u.Email == participationRequest.Email && p.ParticipationTeamId == participationRequest.ParticipationTeamId
                              && p.ParticipationIsDeleted == false
                              select u).FirstOrDefaultAsync();

                if (user != null)
                    throw new AlreadyExistsException("Already in group");

                user = backupUser;

                entity = new Participation
                {
                    ParticipationId = Guid.NewGuid().ToString(),
                    ParticipationUserId = user.Id,
                    ParticipationTeamId = participationRequest.ParticipationTeamId,
                    ParticipationCreatedAt = DateTime.UtcNow,
                    ParticipationIsDeleted = false,
                };
            }
            else
            {
                user = await (from u in _dbContext.User.AsNoTracking()
                              where u.Id == participationRequest.ParticipationUserId
                              select u).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new KeyNotFoundException("No user found");
                }

                var backupUser = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(user));

                user = await (from p in _dbContext.Participation.AsNoTracking()
                              join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                              where u.Id == participationRequest.ParticipationUserId
                              && p.ParticipationTeamId == participationRequest.ParticipationTeamId
                              && p.ParticipationIsDeleted == false
                              select u).FirstOrDefaultAsync();

                if (user != null)
                    throw new AlreadyExistsException("Already in group");

                user = backupUser;
                entity = new Participation
                {
                    ParticipationId = Guid.NewGuid().ToString(),
                    ParticipationUserId = user.Id,
                    ParticipationTeamId = participationRequest.ParticipationTeamId,
                    ParticipationCreatedAt = DateTime.UtcNow,
                    ParticipationIsDeleted = false,
                };
            }


            await _dbContext.Participation.AddAsync(entity);


            var grChatUser = new GroupChatUser
            {
                GroupChatUserId = Guid.NewGuid().ToString(),
                GroupChatUserUserId = user.Id,
                GroupChatUserGroupChatId = participationRequest.ParticipationTeamId,
                GroupChatUserIsDeleted = false,
            };

            await _dbContext.GroupChatUser.AddAsync(grChatUser);

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                await _notiRepo.PushNotiJoinTeam(new Application.DTOs.Team.JoinTeamNotification
                {
                    ActionUserId = participationRequest.ActionUserId,
                    UserId = user.Id,
                    TeamId = participationRequest.ParticipationTeamId,
                });

                var clients = await (from uc in _dbContext.UserConnection.AsNoTracking()
                                     join p in _dbContext.Participation.AsNoTracking()
                                     on uc.UserId equals p.ParticipationUserId
                                     where uc.Type == "app" && p.ParticipationIsDeleted == false
                                     && p.ParticipationTeamId == entity.ParticipationTeamId
                                     select uc.ConnectionId).Distinct().ToListAsync();

                await _hubApp.Clients.Clients(clients).JoinTeam(new
                {
                    UserId = entity.ParticipationUserId,
                    TeamId = entity.ParticipationTeamId,
                });

                return new ParticipationResponse
                {
                    ParticipationId = entity.ParticipationId,
                    ParticipationUserId = entity.ParticipationUserId,
                    ParticipationTeamId = entity.ParticipationTeamId,
                    ParticipationCreatedAt = entity.ParticipationCreatedAt,
                    ParticipationIsDeleted = entity.ParticipationIsDeleted,
                };
            }

            return null;
        }

        public async Task<bool> DeleteParticipation(ParticipationDeleteRequest participationDeleteRequest)
        {
            var entity = await _dbContext.Participation.AsNoTracking().Where(x => x.ParticipationUserId == participationDeleteRequest.UserId
             && x.ParticipationTeamId == participationDeleteRequest.TeamId
             && x.ParticipationIsDeleted == false).FirstOrDefaultAsync();

            if (entity == null)
                throw new KeyNotFoundException("Not found participation");

            entity.ParticipationIsDeleted = true;
            _dbContext.Participation.Update(entity);
            await _dbContext.SaveChangesAsync();

            var gru = await _dbContext.GroupChatUser.Where(
                x => x.GroupChatUserGroupChatId == participationDeleteRequest.TeamId
                && x.GroupChatUserUserId == participationDeleteRequest.UserId
                && x.GroupChatUserIsDeleted == false)
                .FirstOrDefaultAsync();

            if (gru != null)
            {
                gru.GroupChatUserIsDeleted = true;
                _dbContext.Update(gru);
                await _dbContext.SaveChangesAsync();
            }

            var clients = await (from uc in _dbContext.UserConnection.AsNoTracking()
                                 join p in _dbContext.Participation.AsNoTracking()
                                 on uc.UserId equals p.ParticipationUserId
                                 where uc.Type == "app"
                                 && p.ParticipationTeamId == entity.ParticipationTeamId
                                 select uc.ConnectionId).Distinct().ToListAsync();

            await _hubApp.Clients.Clients(clients).LeaveTeam(new
            {
                TeamId = participationDeleteRequest.TeamId,
                UserId = participationDeleteRequest.UserId,
            });

            return true;
        }

        public async Task<List<ParticipationResponse>> GetAllByTeamId(string teamId)
        {
            var entitylist = _dbContext.Participation.AsNoTracking().Where(x => x.ParticipationTeamId == teamId);

            return await entitylist.Select(x => new ParticipationResponse
            {
                ParticipationId = x.ParticipationId,
                ParticipationTeamId = x.ParticipationTeamId,
                ParticipationUserId = x.ParticipationUserId,
                ParticipationCreatedAt = x.ParticipationCreatedAt.FormatTime(),
                ParticipationIsDeleted = x.ParticipationIsDeleted
            }).ToListAsync();
        }

        public async Task<List<string>> GetTeamByUserId(string userId)
        {
            var query = from p in _dbContext.Participation
                        join t in _dbContext.Team on p.ParticipationTeamId equals t.TeamId
                        join u in _dbContext.User on p.ParticipationUserId equals u.Id
                        select new { t, u };

            var outPut = await query.Where(x => x.u.Id == userId).Select(x => x.t.TeamId).ToListAsync();

            return outPut;
        }
    }
}
