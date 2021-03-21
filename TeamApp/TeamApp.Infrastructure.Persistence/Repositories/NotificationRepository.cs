using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Models.Notification;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        public Task<bool> DeleteNotification(string notiId)
        {
            throw new NotImplementedException();
        }

        public Task<List<NotificationResponse>> GetAllByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResponse<NotificationResponse>> GetPaging(RequestParameter parameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReadNotificationSet(string notiId)
        {
            throw new NotImplementedException();
        }
    }
}
