using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.Task;
using TeamApp.Domain.Models.User;
using TeamApp.Infrastructure.Persistence.Entities;
using Task = TeamApp.Infrastructure.Persistence.Entities.Task;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly KhoaLuanContext _dbContext;

        public UserRepository(KhoaLuanContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> AddUser(UserRequest user)
        {
            var userEntity = new User
            {
                UserId = Guid.NewGuid().ToString(),
                UserEmail = user.UserEmail,
                UserPassword = user.UserPassword,
                UserFullname = user.UserFullname,
                UserDateOfBirth = user.UserDateOfBirth,
                UsePhoneNumber = user.UsePhoneNumber,
                UserImageUrl = user.UserImageUrl,
                UserCreatedAt = DateTime.Now,
                UserIsThemeLight = false
            };
            await _dbContext.User.AddAsync(userEntity);
            await _dbContext.SaveChangesAsync();
            return userEntity.UserId;
        }

        public async Task<bool> DeleteUser(string userId)
        {
            return await System.Threading.Tasks.Task.FromResult(false);
        }

        public async Task<List<UserResponse>> GetAll()
        {
            return await _dbContext.User.Select(x => new UserResponse
            {
                UserId = x.UserId,
                UserEmail = x.UserEmail,
                UserPassword = x.UserPassword,
                UserFullname = x.UserFullname,
                UserDateOfBirth = x.UserDateOfBirth,
                UsePhoneNumber = x.UsePhoneNumber,
                UserImageUrl = x.UserImageUrl,
                UserCreatedAt = x.UserCreatedAt,
                UserIsThemeLight = x.UserIsThemeLight
            }).ToListAsync();
        }

        public async Task<List<UserResponse>> GetAllByTeamId(string teamId)
        {
            var query = from p in _dbContext.Participation
                        join t in _dbContext.Team on p.ParticipationTeamId equals t.TeamId
                        join u in _dbContext.User on p.ParticipationUserId equals u.UserId
                        select new { u, t };


            var outPut = await query.Where(x => x.t.TeamId == teamId).Select(x => new UserResponse
            {

                UserId = x.u.UserId,
                UserEmail = x.u.UserEmail,
                UserPassword = x.u.UserPassword,
                UserFullname = x.u.UserFullname,
                UserDateOfBirth = x.u.UserDateOfBirth,
                UsePhoneNumber = x.u.UsePhoneNumber,
                UserImageUrl = x.u.UserImageUrl,
                UserCreatedAt = x.u.UserCreatedAt,
                UserIsThemeLight = x.u.UserIsThemeLight
            }).ToListAsync();

            return outPut;
        }



        public async Task<UserResponse> GetById(string userId)
        {
            var entity = await _dbContext.User.FindAsync(userId);
            if (entity == null)
                return null;
            var userRes = new UserResponse
            {
                UserId = entity.UserId,
                UserEmail = entity.UserEmail,
                UserPassword = entity.UserPassword,
                UserFullname = entity.UserFullname,
                UserDateOfBirth = entity.UserDateOfBirth,
                UsePhoneNumber = entity.UsePhoneNumber,
                UserImageUrl = entity.UserImageUrl,
                UserCreatedAt = entity.UserCreatedAt,
                UserIsThemeLight = entity.UserIsThemeLight
            };
            return userRes;
        }

        public async Task<bool> UpdateUser(string userId, UserRequest user)
        {
            var entity = await _dbContext.User.FindAsync(userId);
            if (entity == null)
                return false;

            entity = new User
            {
                UserId = userId,
                UserEmail = user.UserEmail,
                UserPassword = user.UserPassword,
                UserFullname = user.UserFullname,
                UserDateOfBirth = user.UserDateOfBirth,
                UsePhoneNumber = user.UsePhoneNumber,
                UserImageUrl = user.UserImageUrl,
                UserCreatedAt = user.UserCreatedAt,
                UserIsThemeLight = user.UserIsThemeLight
            };
            _dbContext.User.Update(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
