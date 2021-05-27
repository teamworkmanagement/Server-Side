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

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class ParticipationRepository : IParticipationRepository
    {
        private readonly TeamAppContext _dbContext;
        public ParticipationRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
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
                              where u.Id == participationRequest.ParticipationUserId && p.ParticipationTeamId == participationRequest.ParticipationTeamId
                              select u).FirstOrDefaultAsync();

                if (user != null)
                    throw new AlreadyExistsException("Already in group");

                user = backupUser;
                entity = new Participation
                {
                    ParticipationId = Guid.NewGuid().ToString(),
                    ParticipationUserId = participationRequest.ParticipationUserId,
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

        public async Task<bool> DeleteParticipation(string userId, string teamId)
        {
            var entity = _dbContext.Participation.Where(x => x.ParticipationUserId == userId
            && x.ParticipationTeamId == teamId);

            if (entity.Count() < 0)
                return false;

            var en = entity.ToList()[0];
            en.ParticipationIsDeleted = true;
            _dbContext.Participation.Update(en);
            await _dbContext.SaveChangesAsync();

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
