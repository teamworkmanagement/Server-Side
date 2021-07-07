using System.Collections.Generic;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.User;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserResponse> GetById(string userId);        
        Task<List<UserResponse>> SearchUserNoJoinTeam(string teamId, string keyWord);
        Task<List<UserResponse>> GetAllUserInTeam(string userId, string teamId = null);
        Task<List<UserResponse>> SearchUser(string userId, string keyWord, bool isEmail);
        Task<List<UserResponse>> SearchUserAddToExistsChat(string userId, string teamId, string keyWord, bool isEmail);
        Task<List<UserResponse>> SearchUsersForKanban(UserKanbanSearchModel userKanbanSearch);
        Task<bool> UpdateUserImage(UpdateImageModel updateImageModel);
    }
}
