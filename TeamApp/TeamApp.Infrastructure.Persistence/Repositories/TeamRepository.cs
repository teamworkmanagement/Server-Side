using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.DTOs.Team;
using TeamApp.Application.Utils;
using TeamApp.Application.DTOs.User;
using static TeamApp.Application.Utils.Extensions;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly TeamAppContext _dbContext;
        public TeamRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<TeamResponse> AddTeam(TeamRequest teamReq)
        {
            var teamCode = RadomString.RandomString(6);
            /*bool loop = false;
            while (!loop)
            {
                teamCode = RadomString.RandomString(6);
                var teamCheck = from t in _dbContext.Team.AsNoTracking()
                                where t.TeamCode == teamCode
                                select t;
                if (teamCheck == null)
                    loop = true;
            }*/

            var entity = new Team
            {
                TeamId = Guid.NewGuid().ToString(),
                TeamLeaderId = teamReq.TeamLeaderId,
                TeamName = teamReq.TeamName,
                TeamDescription = teamReq.TeamDescription,
                TeamCreatedAt = DateTime.Now,
                TeamCode = teamCode,
                TeamIsDeleted = false,
            };
            await _dbContext.AddAsync(entity);
            var check = await _dbContext.SaveChangesAsync() > 0;

            if (check)
            {
                await _dbContext.Participation.AddAsync(new Participation
                {
                    ParticipationId = Guid.NewGuid().ToString(),
                    ParticipationUserId = entity.TeamLeaderId,
                    ParticipationTeamId = entity.TeamId,
                    ParticipationCreatedAt = DateTime.UtcNow,
                    ParticipationIsDeleted = false,
                });

                await _dbContext.SaveChangesAsync();

                return new TeamResponse
                {
                    TeamId = entity.TeamId,
                    TeamLeaderId = entity.TeamLeaderId,
                    TeamName = entity.TeamName,
                    TeamDescription = entity.TeamDescription,
                    TeamCreatedAt = entity.TeamCreatedAt,
                    TeamCode = entity.TeamCode,
                };
            }
            return null;
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
                TeamCreatedAt = entity.TeamCreatedAt.FormatTime(),
                TeamCode = entity.TeamCode,
                TeamIsDeleted = entity.TeamIsDeleted,
            };
        }

        public async Task<List<TeamResponse>> GetByUserId(string userId)
        {
            var query = from p in _dbContext.Participation.AsNoTracking()
                        join t in _dbContext.Team.AsNoTracking() on p.ParticipationTeamId equals t.TeamId
                        join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                        select new { t, u };

            var outPut = await query.Where(x => x.u.Id == userId).Select(entity => new TeamResponse
            {
                TeamId = entity.t.TeamId,
                TeamLeaderId = entity.t.TeamLeaderId,
                TeamName = entity.t.TeamName,
                TeamDescription = entity.t.TeamDescription,
                TeamCreatedAt = entity.t.TeamCreatedAt.FormatTime(),
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

        public async Task<List<UserResponse>> GetAllByTeamId(string teamId)
        {
            var query = from p in _dbContext.Participation.AsNoTracking()
                        join t in _dbContext.Team.AsNoTracking() on p.ParticipationTeamId equals t.TeamId
                        join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                        where t.TeamId == teamId
                        select u;

            var outPut = await query.Select(u => new UserResponse
            {
                UserId = u.Id,
                UserEmail = u.Email,
                UserFullname = u.FullName,
                UserDateOfBirth = u.Dob,
                UsePhoneNumber = u.PhoneNumber,
                UserImageUrl = u.ImageUrl,
                UserCreatedAt = u.CreatedAt
            }).ToListAsync();


            return outPut;
        }

        public async Task<TeamResponse> JoinTeam(JoinTeamRequest request)
        {
            var team = await (from t in _dbContext.Team.AsNoTracking()
                              where t.TeamCode == request.TeamCode
                              select t).FirstOrDefaultAsync();
            if (team == null)
                return null;


            team = await (from t in _dbContext.Team.AsNoTracking()
                          join p in _dbContext.Participation.AsNoTracking() on t.TeamId equals p.ParticipationTeamId
                          where t.TeamCode == request.TeamCode && p.ParticipationUserId == request.UserId
                          select t).FirstOrDefaultAsync();

            if (team != null)
                return new TeamResponse
                {
                    TeamId = team.TeamId,
                };
            await _dbContext.Participation.AddAsync(new Participation
            {
                ParticipationId = Guid.NewGuid().ToString(),
                ParticipationUserId = request.UserId,
                ParticipationTeamId = team.TeamId,
                ParticipationCreatedAt = DateTime.UtcNow,
                ParticipationIsDeleted = false,
            });

            team = await (from t in _dbContext.Team.AsNoTracking()
                          where t.TeamCode == request.TeamCode
                          select t).FirstOrDefaultAsync();

            return new TeamResponse
            {
                TeamId = team.TeamId,
                TeamLeaderId = team.TeamLeaderId,
                TeamName = team.TeamName,
                TeamDescription = team.TeamDescription,
                TeamCreatedAt = team.TeamCreatedAt,
                TeamCode = team.TeamCode,
            };
        }
    }
}
