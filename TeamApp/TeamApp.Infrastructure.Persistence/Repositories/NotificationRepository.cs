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
using Microsoft.AspNetCore.SignalR;
using TeamApp.Infrastructure.Persistence.Hubs.Notification;
using System.Collections.ObjectModel;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IFirebaseMessagingService _firebaseMessagingService;

        private readonly IHubContext<HubNotificationClient, IHubNotificationClient> _notiHub;
        public NotificationRepository(TeamAppContext dbContext, IFirebaseMessagingService firebaseMessagingService, IHubContext<HubNotificationClient, IHubNotificationClient> notiHub)
        {
            _dbContext = dbContext;
            _firebaseMessagingService = firebaseMessagingService;
            _notiHub = notiHub;
        }

        public async Task<PagedResponse<NotificationResponse>> GetPaging(NotificationRequestParameter parameter)
        {
            var query = from n in _dbContext.Notification.AsNoTracking()
                        join u in _dbContext.User.AsNoTracking() on n.NotificationUserId equals u.Id
                        orderby n.NotificationCreatedAt descending
                        where n.NotificationUserId == parameter.UserId
                        select n;
            var totalRecords = await query.CountAsync();
            query = query.AsNoTracking().Skip(parameter.SkipItems).Take(parameter.PageSize);

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

            var outPut = new PagedResponse<NotificationResponse>(entityList, parameter.PageSize, totalRecords, skipRows: parameter.SkipItems);

            return outPut;
        }

        public async Task<string> PushNoti(string token, string title, string body)
        {
            var clientLists = await (from uc in _dbContext.UserConnection.AsNoTracking()
                                     where uc.Type == "notification"
                                     select uc.ConnectionId).ToListAsync();


            Console.WriteLine("count = " + clientLists.Count());
            var readOnlyList = new ReadOnlyCollection<string>(clientLists);
            await _notiHub.Clients.Clients(readOnlyList).SendNoti(new
            {
                Name = "Dunxg Nguyeenx",
                Id = 10,
            });

            //await _notiHub.Clients.All.SendNoti();

            return "abc";
            //return await _firebaseMessagingService.SendNotification(token, title, body);
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
