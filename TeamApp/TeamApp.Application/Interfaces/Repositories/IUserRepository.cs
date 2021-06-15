using System.Collections.Generic;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.User;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserResponse> GetById(string userId);
        Task<bool> UpdateUser(string userId, UserRequest userReq);
        Task<bool> DeleteUser(string userId);
        /// <summary>
        /// keyWord can be email or name
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        Task<List<UserResponse>> SearchUserNoJoinTeam(string teamId, string keyWord);
        /// <summary>
        /// Get user in team of user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        Task<List<UserResponse>> GetAllUserInTeam(string userId, string teamId = null);
        Task<List<UserResponse>> SearchUser(string userId, string keyWord, bool isEmail);
        Task<List<UserResponse>> SearchUserAddToExistsChat(string userId, string teamId, string keyWord, bool isEmail);
        Task<List<UserResponse>> SearchUsersForKanban(UserKanbanSearchModel userKanbanSearch);

        Task<bool> UpdateUserImage(UpdateImageModel updateImageModel);
    }
}
