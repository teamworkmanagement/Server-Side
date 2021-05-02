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
using TeamApp.Application.DTOs.Notification;
using TeamApp.Application.Utils;
using TeamApp.Application.Interfaces;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IFirebaseMessagingService _firebaseMessagingService;
        public NotificationRepository(TeamAppContext dbContext, IFirebaseMessagingService firebaseMessagingService)
        {
            _dbContext = dbContext;
            _firebaseMessagingService = firebaseMessagingService;
        }
        public async Task<bool> DeleteNotification(string notiId)
        {
            var entity = await _dbContext.Notification.FindAsync(notiId);
            if (entity == null)
                return false;

            entity.NotificationIsDeleted = true;
            _dbContext.Notification.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<NotificationResponse>> GetAllByUserId(string userId)
        {
            var query = from n in _dbContext.Notification
                        where n.NotificationUserId == userId
                        select n;

            return await query.Select(x => new NotificationResponse
            {
                NotificationId = x.NotificationId,
                NotificationUserId = x.NotificationUserId,
                NotificationContent = x.NotificationContent,
                NotificationCreatedAt = x.NotificationCreatedAt.FormatTime(),
                NotificationLink = x.NotificationLink,
                NotificationStatus = x.NotificationStatus,
                NotificationIsDeleted = x.NotificationIsDeleted,
            }).ToListAsync();
        }

        public async Task<PagedResponse<NotificationResponse>> GetPaging(RequestParameter parameter)
        {
            var query = _dbContext.Notification.Skip(parameter.PageSize * parameter.PageNumber).Take(parameter.PageSize);

            var entityList = await query.Select(x => new NotificationResponse
            {
                NotificationId = x.NotificationId,
                NotificationUserId = x.NotificationUserId,
                NotificationContent = x.NotificationContent,
                NotificationCreatedAt = x.NotificationCreatedAt.FormatTime(),
                NotificationLink = x.NotificationLink,
                NotificationStatus = x.NotificationStatus,
                NotificationIsDeleted = x.NotificationIsDeleted,
            }).ToListAsync();

            var outPut = new PagedResponse<NotificationResponse>(entityList, parameter.PageNumber, parameter.PageSize, await query.CountAsync());

            return outPut;
        }

        public async Task<string> PushNoti(string token, string title, string body)
        {
            return await _firebaseMessagingService.SendNotification(token, title, body);
        }

        public async Task<bool> ReadNotificationSet(string notiId)
        {
            var entity = await _dbContext.Notification.FindAsync(notiId);
            if (entity == null)
                return false;

            entity.NotificationStatus = true;
            _dbContext.Notification.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
