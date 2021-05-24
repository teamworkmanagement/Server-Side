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

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TeamAppContext _dbContext;

        public UserRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<bool> DeleteUser(string userId)
        {
            return await System.Threading.Tasks.Task.FromResult(false);
        }

        public async Task<List<UserResponse>> GetAllUserInTeam(string userId, string teamId = null)
        {
            if (teamId != null)
            {
                var user = (from t in _dbContext.Team.AsNoTracking()
                            join p in _dbContext.Participation.AsNoTracking() on t.TeamId equals p.ParticipationTeamId
                            join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                            where t.TeamId == teamId
                            select new { u.Id, u.FullName, u.ImageUrl }).Distinct();

                return await user.Select(x => new UserResponse
                {
                    UserId = x.Id,
                    UserFullname = x.FullName,
                    UserImageUrl = x.ImageUrl,
                }).ToListAsync();
            }

            else
            {
                var teamList = from t in _dbContext.Team.AsNoTracking()
                               join par in _dbContext.Participation.AsNoTracking() on t.TeamId equals par.ParticipationTeamId
                               join u in _dbContext.User.AsNoTracking() on par.ParticipationUserId equals u.Id
                               select new { u, t.TeamId, t.TeamName };

                //danh sách team mà user join
                teamList = teamList.Where(x => x.u.Id == userId).Distinct();

                var queryUser = await (from listTeam in teamList.AsNoTracking()
                                       join p in _dbContext.Participation.AsNoTracking() on listTeam.TeamId equals p.ParticipationTeamId
                                       join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                                       select new { u.Id, u.FullName, u.ImageUrl }).Distinct().ToListAsync();

                return queryUser.Select(x => new UserResponse
                {
                    UserId = x.Id,
                    UserFullname = x.FullName,
                    UserImageUrl = x.ImageUrl,
                }).ToList();
            }
        }

        public async Task<UserResponse> GetById(string userId)
        {
            var entity = await _dbContext.User.FindAsync(userId);
            if (entity == null)
                return null;
            var userRes = new UserResponse
            {

            };
            return userRes;
        }

        public async Task<List<UserResponse>> SearchUser(string userId, string keyWord)
        {
            var query = "SELECT * FROM user " +
                $"where user.user_id <> '{userId}' and user.user_fullname like '%{keyWord}%'";
            Console.WriteLine(query);

            var outPut = await _dbContext.User.FromSqlRaw(query).ToListAsync();

            return outPut.Select(x => new UserResponse
            {
                UserId = x.Id,
                UserEmail = x.Email,
                UserFullname = x.FullName,
                UserImageUrl = string.IsNullOrEmpty(x.ImageUrl) ? $"https://ui-avatars.com/api/?name={x.FullName}" : x.ImageUrl,
            }).ToList();
        }

        public async Task<List<UserResponse>> SearchUserNoJoinTeam(string teamId, string keyWord)
        {
            var query = await (from p in _dbContext.Participation.AsNoTracking()
                               join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                               where p.ParticipationTeamId != teamId
                               select u).Distinct().AsNoTracking().ToListAsync();

            keyWord = keyWord.UnsignUnicode();

            if (!string.IsNullOrEmpty(keyWord))
                query = query.Where(x => x.FullName.UnsignUnicode().Contains(keyWord) || x.Email.UnsignUnicode().Contains(keyWord)).ToList();

            var outPut = query.Select(x => new UserResponse
            {
                UserId = x.Id,

                UserFullname = x.FullName,

                UserImageUrl = x.ImageUrl,

            }).ToList();

            return outPut;
        }

        public async Task<bool> UpdateUser(string userId, UserRequest user)
        {
            var entity = await _dbContext.User.FindAsync(userId);
            if (entity == null)
                return false;


            _dbContext.User.Update(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
