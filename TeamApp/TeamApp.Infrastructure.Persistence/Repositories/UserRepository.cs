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
                
            };
            await _dbContext.User.AddAsync(userEntity);
            await _dbContext.SaveChangesAsync();
            return "";
        }

        public async Task<bool> DeleteUser(string userId)
        {
            return await System.Threading.Tasks.Task.FromResult(false);
        }

        public async Task<List<UserResponse>> GetAll()
        {
            return await _dbContext.User.Select(x => new UserResponse
            {
                
            }).ToListAsync();
        }

        public async Task<List<UserResponse>> GetAllByTeamId(string teamId)
        {
            var query = from p in _dbContext.Participation
                        join t in _dbContext.Team on p.ParticipationTeamId equals t.TeamId
                        join u in _dbContext.User on p.ParticipationUserId equals u.Id
                        select new { u, t };


            var outPut = await query.Where(x => x.t.TeamId == teamId).Select(x => new UserResponse
            {

                
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
               
            };
            return userRes;
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
