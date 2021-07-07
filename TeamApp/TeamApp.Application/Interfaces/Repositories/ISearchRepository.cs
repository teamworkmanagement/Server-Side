using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.AppSearch;
using TeamApp.Application.Wrappers;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface ISearchRepository
    {
        Task<PagedResponse<TaskSearchResponse>> SearchTasks(AppSearchRequest appSearchRequest);
        Task<PagedResponse<TeamSearchResponse>> SearchTeams(AppSearchRequest appSearchRequest);
        Task<PagedResponse<BoardSearchResponse>> SearchBoards(AppSearchRequest appSearchRequest);
        Task<PagedResponse<GroupChatSearchResponse>> SearchGroupChats(AppSearchRequest appSearchRequest);
    }
}
