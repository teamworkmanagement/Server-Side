using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Domain.Models;
using TeamApp.Domain.Models.Task;
using TeamApp.Domain.Models.User;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        //for test
        Task<List<UserResponse>> GetAll();
        Task<List<UserResponse>> GetAllByTeamId(string teamId);
        Task<UserResponse> GetById(string userId);
        Task<string> AddUser(UserRequest userReq);
        Task<bool> UpdateUser(string userId, UserRequest userReq);
        Task<bool> DeleteUser(string userId);
    }
}
