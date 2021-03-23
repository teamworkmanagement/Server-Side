using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.DTOs.Message;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly KhoaLuanContext _dbContext;

        public MessageRepository(KhoaLuanContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> AddMessage(MessageRequest msgReq)
        {
            var entity = new Message
            {
                MessageId = new Guid().ToString(),
                MessageUserId = msgReq.MessageUserId,
                MessageGroupChatId = msgReq.MessageGroupChatId,
                MessageContent = msgReq.MessageContent,
                MessageCreatedAt = DateTime.UtcNow,
                MessageIsDeleted = msgReq.MessageIsDeleted,
            };

            await _dbContext.Message.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.MessageId;
        }

        public async Task<bool> DeleteMessage(string msgId)
        {
            var entity = await _dbContext.Message.FindAsync(msgId);
            if (entity == null)
                return false;

            entity.MessageIsDeleted = true;
            _dbContext.Message.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<MessageResponse>> GetAllByUserId(string userId)
        {
            var query = from m in _dbContext.Message
                        where m.MessageUserId == userId
                        select m;

            return await query.Select(x => new MessageResponse
            {
                MessageId = x.MessageId,
                MessageUserId = x.MessageUserId,
                MessageGroupChatId = x.MessageGroupChatId,
                MessageContent = x.MessageContent,
                MessageCreatedAt = x.MessageCreatedAt,
                MessageIsDeleted = x.MessageIsDeleted,
            }).ToListAsync();
        }

        public async Task<PagedResponse<MessageResponse>> GetPaging(RequestParameter parameter)
        {
            var query = _dbContext.Message.Skip(parameter.PageNumber * parameter.PageSize).Take(parameter.PageSize);

            var items = await query.Select(x => new MessageResponse
            {
                MessageId = x.MessageId,
                MessageUserId = x.MessageUserId,
                MessageGroupChatId = x.MessageGroupChatId,
                MessageContent = x.MessageContent,
                MessageCreatedAt = x.MessageCreatedAt,
                MessageIsDeleted = x.MessageIsDeleted,
            }).ToListAsync();

            return new PagedResponse<MessageResponse>(items, parameter.PageNumber, parameter.PageSize, items.Count());
        }
    }
}
