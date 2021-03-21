using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Models.Message;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        public Task<string> AddMessage(MessageRequest msgReq)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteMessage(string msgId)
        {
            throw new NotImplementedException();
        }

        public Task<List<MessageResponse>> GetAllByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResponse<MessageResponse>> GetPaging(RequestParameter parameter)
        {
            throw new NotImplementedException();
        }
    }
}
