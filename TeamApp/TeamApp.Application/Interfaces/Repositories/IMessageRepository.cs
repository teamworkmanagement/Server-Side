using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Message;
using TeamApp.Application.Filters;
using TeamApp.Application.Wrappers;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IMessageRepository
    {
        Task<List<MessageResponse>> GetAllByUserId(string userId);
        Task<PagedResponse<MessageResponse>> GetPaging(MessageRequestParameter parameter);
        Task<string> AddMessage(MessageRequest msgReq);
        Task<bool> DeleteMessage(string msgId);
    }
}
