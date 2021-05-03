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
