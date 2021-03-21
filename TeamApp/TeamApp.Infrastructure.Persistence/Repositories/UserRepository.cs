using Microsoft.EntityFrameworkCore;
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

        public Task<string> AddUser(UserRequest user)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteUser(string userId)
        {
            throw new System.NotImplementedException();
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

        public Task<List<UserResponse>> GetAllByTeamId(string teamId)
        {
            throw new System.NotImplementedException();
        }

        public Task<UserResponse> GetById(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateUser(string userId, UserRequest user)
        {
            throw new System.NotImplementedException();
        }
    }
}
