using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Paricipation;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class ParticipationRepository : IParticipationRepository
    {
        private readonly KhoaLuanContext _dbContext;
        public ParticipationRepository(KhoaLuanContext dbContext)
        {
            _dbContext = dbContext;
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

        public Task<List<ParticipationResponse>> GetAllByTeamId(string teamId)
        {
            var entitylist = _dbContext.Participation.Where(x => x.ParticipationTeamId == teamId);

            return entitylist.Select(x => new ParticipationResponse
            {
                ParticipationId = x.ParticipationId,
                ParticipationTeamId = x.ParticipationTeamId,
                ParticipationUserId = x.ParticipationUserId,
                ParticipationCreatedAt = x.ParticipationCreatedAt,
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
