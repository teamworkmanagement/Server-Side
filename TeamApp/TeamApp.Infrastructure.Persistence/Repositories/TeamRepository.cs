using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.Team;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly KhoaLuanContext _dbContext;
        public TeamRepository(KhoaLuanContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> AddTeam(TeamRequest teamReq)
        {
            var entity = new Team
            {
                TeamId = new Guid().ToString(),
                TeamLeaderId = teamReq.TeamLeaderId,
                TeamName = teamReq.TeamName,
                TeamDescription = teamReq.TeamDescription,
                TeamCreatedAt = DateTime.Now,
                TeamCode = teamReq.TeamCode,
                TeamIsDeleted = false,
            };
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.TeamId;
        }

        public async Task<bool> DeleteTeam(string teamId)
        {
            var entity = await _dbContext.Team.FindAsync(teamId);
            if (entity == null)
                return false;

            entity.TeamIsDeleted = true;
            _dbContext.Team.Update(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<TeamResponse> GetById(string teamId)
        {
            var entity = await _dbContext.Team.FindAsync(teamId);
            if (entity == null)
                return null;

            return new TeamResponse
            {
                TeamId = entity.TeamId,
                TeamLeaderId = entity.TeamLeaderId,
                TeamName = entity.TeamName,
                TeamDescription = entity.TeamDescription,
                TeamCreatedAt = entity.TeamCreatedAt,
                TeamCode = entity.TeamCode,
                TeamIsDeleted = entity.TeamIsDeleted,
            };
        }

        public async Task<List<TeamResponse>> GetByUserId(string userId)
        {
            var query = from p in _dbContext.Participation
                        join t in _dbContext.Team on p.ParticipationTeamId equals t.TeamId
                        join u in _dbContext.User on p.ParticipationUserId equals u.Id
                        select new { t, u };

            var outPut = await query.Where(x => x.u.Id == userId).Select(entity => new TeamResponse
            {
                TeamId = entity.t.TeamId,
                TeamLeaderId = entity.t.TeamLeaderId,
                TeamName = entity.t.TeamName,
                TeamDescription = entity.t.TeamDescription,
                TeamCreatedAt = entity.t.TeamCreatedAt,
                TeamCode = entity.t.TeamCode,
                TeamIsDeleted = entity.t.TeamIsDeleted,
            }).ToListAsync();

            return outPut;
        }

        public async Task<bool> UpdateTeam(string teamId, TeamRequest teamReq)
        {
            var entity = await _dbContext.Team.FindAsync(teamId);
            if (entity == null)
                return false;

            entity = new Team
            {
                TeamId = teamId,
                TeamLeaderId = teamReq.TeamLeaderId,
                TeamName = teamReq.TeamName,
                TeamDescription = teamReq.TeamDescription,
                TeamCreatedAt = teamReq.TeamCreatedAt,
                TeamCode = teamReq.TeamCode,
                TeamIsDeleted = teamReq.TeamIsDeleted
            };

            _dbContext.Team.Update(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
