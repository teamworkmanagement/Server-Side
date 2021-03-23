using System.Collections.Generic;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.User;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        //for test
        Task<List<UserResponse>> GetAllByTeamId(string teamId);
        Task<UserResponse> GetById(string userId);
        Task<bool> UpdateUser(string userId, UserRequest userReq);
        Task<bool> DeleteUser(string userId);
    }
}
